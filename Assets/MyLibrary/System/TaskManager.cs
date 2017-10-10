using System;
using System.Collections.Generic;
using System.Diagnostics;

/// <summary>
/// タスクマネージャ
/// </summary>
public class TaskManager : SingletonMonoBehaviour<TaskManager>
{
	/// <summary>
	/// 外部公開用タスクグループインターフェイス
	/// </summary>
	public interface ITaskGroup
	{
		/// <summary>
		/// タスク追加
		/// </summary>
		/// <param name="action">実行関数</param>
		void AddTask(Action<Action> action);

		/// <summary>
		/// タスク実行開始
		/// </summary>
		/// <param name="endAction">全てのタスク実行が終了した時に呼び出される関数。EndActionで登録するのと同じ。</param>
		/// <returns>実行開始出来た場合にtrueを返す</returns>
		bool Run(Action endAction = null);

		/// <summary>
		/// タスク実行をキャンセル
		/// </summary>
		/// <remarks> 
		/// Run()を実行後、タスク処理をすべてキャンセルしたい場合に使用する。 
		/// まだ実行されていないタスクは開始されずに破棄され、Run()で指定した onEnd コールバックが呼ばれる。
		/// </remarks>
		void Cancel();

		/// <summary>
		/// 実行中かどうか
		/// </summary>
		bool IsRun();

		/// <summary>
		/// 全てのタスク実行が終了したかどうか
		/// </summary>
		bool IsEnd();

		/// <summary>
		/// 名前
		/// </summary>
		string name	{ get; set; }
	}

	//=============================================================================
	/// <summary>
	/// タスクのグループ化用クラス
	/// </summary>
	private class TaskGroup : ITaskGroup
	{
		public delegate bool TaskFunction();
		public delegate bool TaskFunction<T>(T userParam);

		//--------------------------------------------------------
		//	定数
		//--------------------------------------------------------
		private const int DefaultCapacity	= 1;
		// 状態
		private enum GroupStatus {
			None,
			Initialized,
			Run,
			End
		}
		//--------------------------------------------------------
		//	メンバ変数
		//--------------------------------------------------------
		private	readonly bool			mIsAsync;									//!< 非同期処理を行うかどうか
		private List<Task>				mTaskList;
		private Action					mEndAction	= delegate{};
	
		private GroupStatus				mStatus;

		//--------------------------------------------------------
		//	プロパティ
		//--------------------------------------------------------
		/// <summary>
		/// 全てのタスク実行が終了した時に呼び出される関数
		/// </summary>
		public Action EndAction {
			get { return mEndAction; }
			set { mEndAction = value; }
		}

		/// <summary>
		/// プライオリティ
		/// </summary>
		public uint				Priority { get; private set; }

		/// <summary>
		/// 名前
		/// </summary>
		public string			name	{ get; set; }
		//--------------------------------------------------------
		//	public メンバ関数
		//		TaskManagerから使用
		//--------------------------------------------------------
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public TaskGroup() :
			this(false, DefaultCapacity, TaskManager.DefaultPriority)
		{
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="isAsync">非同期実行かどうか</param>
		/// <param name="capacity">登録するタスクの数</param>
		/// <param name="priority">優先度</param>
		public TaskGroup(bool isAsync, int capacity, uint priority)
		{
			mIsAsync	= isAsync;
			mStatus		= GroupStatus.Initialized;
			Priority	= priority;
			mTaskList	= new List<Task>(capacity);
		}

		/// <summary>
		/// 更新処理
		/// </summary>
		public void UpdateGroup()
		{
			if (mStatus != GroupStatus.Run) {
				return;
			}

			int count = mTaskList.Count;
			bool allEnd = true;
			for (int i = 0 ; i < count ; i++) {
				Task task = mTaskList[i];
				if (task == null) {
					break;
				}

				if (task.Status == Task.StatusCode.SetAction) {
					// タスクアクション呼び出し
					task.CallAction();
				}
				if (task.Status == Task.StatusCode.ActionCalled) {	// アクション実行したそのフレームで終了される事があるのでelse ifにはしない
					allEnd = false;
					if (!mIsAsync) {
						break;	// 同期実行の時は１つしか実行状態にならない
					}
				}
			}
			if (allEnd) {
				End();
			}
		}

		/// <summary>
		/// 状態ごとのタスクの数を取得
		/// </summary>
		public void GetActiveCount(out int setActionCount, out int calledCount, out int endedCount)
		{
			setActionCount = 0;
			calledCount = 0;
			endedCount = 0;

			int count = mTaskList.Count;
			for (int i = 0 ; i < count ; i++) {
				Task task = mTaskList[i];
				if (task == null) {
					break;
				}
				switch (task.Status) {
				case Task.StatusCode.SetAction:
					setActionCount ++;
					break;
				case Task.StatusCode.ActionCalled:
					calledCount ++;
					break;
				case Task.StatusCode.End:
					endedCount ++;
					break;
				}
			}
		}

		/// <summary>
		/// 終了処理
		/// </summary>
		private void End()
		{
			if (EndAction != null) {
				EndAction();
				EndAction = null;
			}
			mStatus = GroupStatus.End;
			mTaskList.Clear();
			mTaskList = null;
		}

		//--------------------------------------------------------
		//	ITaskGroup 関数
		//--------------------------------------------------------
		/// <summary>
		/// タスク追加
		/// </summary>
		/// <param name="action">実行関数</param>
		public void AddTask(Action<Action> action)
		{
			DebugUtil.Assert(mStatus == GroupStatus.Initialized);
			DebugUtil.NullAssert(action);

			mTaskList.Add(new Task(action));
		}

		/// <summary>
		/// タスク実行開始
		/// </summary>
		/// <param name="endAction">全てのタスク実行が終了した時に呼び出される関数。EndActionで登録するのと同じ。</param>
		/// <returns>実行開始出来た場合にtrueを返す</returns>
		public bool Run(Action endAction = null)
		{
			if (mStatus != GroupStatus.Initialized) {
				return false;
			}
			EndAction = endAction;
			mStatus = GroupStatus.Run;
			return true;
		}

		/// <summary>
		/// タスク実行をキャンセル
		/// </summary>
		/// <remarks> 
		/// Run()を実行後、タスク処理をすべてキャンセルしたい場合に使用する。 
		/// まだ実行されていないタスクは開始されずに破棄され、Run()で指定した onEnd コールバックが呼ばれる。
		/// </remarks>
		public void Cancel() {
			this.End();
		}

		/// <summary>
		/// 実行中かどうか
		/// </summary>
		public bool IsRun() 
		{
			 return mStatus == GroupStatus.Run;
		}
		/// <summary>
		/// 全てのタスク実行が終了したかどうか
		/// </summary>
		public bool IsEnd() 
		{
			return mStatus == GroupStatus.End;
		}

	} // class TaskGroup

	//=============================================================================
	/// <summary>
	/// 通常のタスク
	/// </summary>
	private class Task
	{
		public enum StatusCode {
			SetAction,		// アクションが設定された
			ActionCalled,	// アクションが呼び出された
			End				// 終了通知を受けた
		}

		private Action<Action>	mAction;
		/// <summary>
		/// 状態
		/// </summary>
		public StatusCode Status { get; private set; }

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public Task(Action<Action> action)
		{
			mAction = action;
			Status = StatusCode.SetAction;
		}

		/// <summary>
		/// 更新
		/// </summary>
		public void CallAction()
		{
			DebugUtil.Assert(Status == StatusCode.SetAction);
			Status = StatusCode.ActionCalled;
			mAction(OnEnded);
		}

		/// <summary>
		/// 終了時に呼ばれる
		/// </summary>
		private void OnEnded()
		{
			if (Status == StatusCode.ActionCalled) {
				Status = StatusCode.End;
			}
		}
	}	// class Task
	
	/// <summary>
	/// タスクグループデフォルトプライオリティ
	/// </summary>
	public const uint DefaultPriority	= 100;
	//--------------------------------------------------------
	//	メンバ変数
	//--------------------------------------------------------
	private LinkedList<TaskGroup>		mTaskGroupList	= new LinkedList<TaskGroup>();		//!< 外部から登録されたグループリスト
	private TaskGroup					mDefaultGroup;										//!< 常に保持しているデフォルトグループ

	//--------------------------------------------------------
	//	private メンバ関数
	//--------------------------------------------------------
	/// <summary>
	/// グループをリストに追加
	/// </summary>
	/// <param name="newGroup">追加するグループ</param>
	private void AddGroup(TaskGroup newGroup)
	{
		LinkedListNode<TaskGroup> node = mTaskGroupList.First;

		while (node != null) {
			TaskGroup group = node.Value;
			if (newGroup.Priority < group.Priority) {
				mTaskGroupList.AddBefore(node, newGroup);
				return;
			}
			node = node.Next;
		}
		mTaskGroupList.AddLast(newGroup);
	}

	//--------------------------------------------------------
	//	public メンバ関数
	//--------------------------------------------------------
	/// <summary>
	/// デフォルトコンストラクタ
	/// </summary>
	public TaskManager() :
		this(10)
	{
	}

	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="defaultGroupCapacity">デフォルトグループのキャパシティ</param>
	public TaskManager(int defaultGroupCapacity)
	{
		// デフォルトグループとして1つ登録
		mDefaultGroup = CreateSyncTaskGroup(defaultGroupCapacity, DefaultPriority) as TaskGroup;
	}

	/// <summary>
	/// 更新
	/// </summary>
	public void Update()
	{
		LinkedListNode<TaskGroup> node = mTaskGroupList.First;
		while (node != null) {
			TaskGroup group = node.Value;
			var nextNode = node.Next;

			group.UpdateGroup();
			if (group.IsEnd()) {
				mTaskGroupList.Remove(group);
			}

			node = nextNode;
		}
	}


	/// <summary>
	/// 同期タスクグループを作成する
	/// </summary>
	/// <param name="capacity">登録タスクキャパシティ</param>
	/// <param name="priority">プライオリティ(小さい方が先に呼ばれる。同じプライオリティでは先に登録されている方が先に呼ばれる)</param>
	/// <returns>作成したタスクグループインターフェイス</returns>
	public ITaskGroup CreateSyncTaskGroup(int capacity = 8, uint priority = DefaultPriority)
	{
		TaskGroup group = new TaskGroup(false, capacity, priority);
		AddGroup(group);
		return group;
	}

	/// <summary>
	/// 非同期タスクグループを作成する
	/// </summary>
	/// <param name="capacity">登録タスクキャパシティ</param>
	/// <returns>作成したタスクグループインターフェイス</returns>
	public ITaskGroup CreateAsyncTaskGroup(int capacity = 8, uint priority = DefaultPriority)
	{
		TaskGroup group = new TaskGroup(true, capacity, priority);
		AddGroup(group);
		return group;
	}

	/// <summary>
	/// タスクをダンプ
	/// </summary>
	[Conditional("DEBUG")]
	public void Dump()
	{
		DebugUtil.Log("===== TaskManager Dump =====");
		LinkedListNode<TaskGroup> node = mTaskGroupList.First;
		while (node != null) {
			TaskGroup group = node.Value;
			
			int setActionCount, calledCount, endedCount;
			group.GetActiveCount(out setActionCount, out calledCount, out endedCount);

			DebugUtil.Log(string.Format("Group Name:{0} Priority:{1} ReadyTask:{2} WaitTask:{3} EndTask:{4}", group.name, group.Priority, setActionCount, calledCount, endedCount));

			node = node.Next;
		}		
	}
}
