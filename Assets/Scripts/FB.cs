using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;

public class FB : MonoBehaviour//Leaderboard : MonoBehaviour //13번까지진행하면, 파이어베이스 하기전 로컬 사용한다. 리더보드 파이어베이스로 대치한다.
{
    private class RankingInfo
    {
        public int score;
        public string nickname;
        public string timestamp;

        public RankingInfo(int score, string nickname, string timestamp)
        {
            this.score = score;
            this.nickname = nickname;
            this.timestamp = timestamp;
        }

        public RankingInfo(Dictionary<string, object> dic)
        {
            FromDictionary(dic);
        }

        public Dictionary<string, object> ToDictionary()
        {
            var dic = new Dictionary<string, object>();
            dic["score"] = score;
            dic["nickname"] = nickname;
            dic["timestamp"] = timestamp;
            return dic;
        }

        public void FromDictionary(Dictionary<string, object> dic)
        {
            score = int.Parse(dic["score"].ToString());
            nickname = (string)dic["nickname"];
            timestamp = (string)dic["timestamp"];
        }
    }

    private List<RankingInfo> rankingInfoList = new List<RankingInfo>();
    public List<Ranking> rankingList;

    private const string ScoreCollectionName = "Scores";
    private static DatabaseReference ScoresReference;

    private void Awake()
    {
        ScoresReference = FirebaseDatabase.DefaultInstance.GetReference(ScoreCollectionName);
    }

    private IEnumerator Start()
    {
        for (; ; )
        {
            yield return new WaitForSeconds(0.5f);
            RefreshUI();
        }
    }

    private void RefreshUI()
    {
        foreach (var r in rankingList)
            r.Reset();
        for (int i = 0; i < rankingList.Count && i < rankingInfoList.Count; ++i)
        {
            rankingList[i].Score = rankingInfoList[i].score;
            rankingList[i].Nickname = rankingInfoList[i].nickname;
            rankingList[i].Timestamp = rankingInfoList[i].timestamp;
        }
    }

    public void AddScore(int score, string nickname)
    {
        AddScoreToLeaders(score, nickname, DateTime.Now.ToString("yy/MM/dd hh:mm:ss"));
    }

    private void GetScores()
    {
        GetScores(
        () =>
        {
            Debug.Log("Faulted");
        },
(snapshot) =>
{
    Debug.Log("Completed");
    var scores = snapshot.Value as List<object>;
    var rankingList = new List<RankingInfo>();
    foreach (var s in scores)
        rankingList.Add(new RankingInfo(s as Dictionary<string, object>));
});
    }

    private static void GetScores(Action OnFaulted, Action<DataSnapshot> OnCompleted)
    {
        ScoresReference.GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                OnFaulted?.Invoke();
            }
            else if (task.IsCompleted)
            {
                OnCompleted?.Invoke(task.Result);
            }
        });
    }

    private static void WriteNewScore(int score, string nickname, string timestamp)
    {
        // 엔트리 하나 추가
        var key = ScoresReference.Push().Key;

        // 추가한 엔트리의 데이터를 수정할 것을 조립
        var entry = new RankingInfo(score, nickname, timestamp);
        var entryValues = entry.ToDictionary();
        var childUpdates = new Dictionary<string, object>();
        childUpdates["/Scores/" + key] = entryValues;

        // 실제 디비에 데이터 업데이트 요청
        ScoresReference.UpdateChildrenAsync(childUpdates);
    }

    private void AddScoreToLeaders(int score, string nickname, string timestamp)
    {
        ScoresReference.RunTransaction(mutableData =>
        {
            // 서버에 있는 점수 리스트
            var leaders = mutableData.Value as List<object>;
            // 점수 리스트가 없으면, 생성
            if (leaders == null)
            {
                leaders = new List<object>();
            }
            // 점수 리스트가 최대 개수를 넘었으면, 가장 작은 점수를 제거
            else if (mutableData.ChildrenCount >= rankingList.Count)
            {
                var minScore = long.MaxValue;
                object minVal = null;
                foreach (var child in leaders)
                {
                    var childDic = child as Dictionary<string, object>;
                    if (childDic == null)
                        continue;
                    var rankingEntry = new RankingInfo(childDic);
                    if (rankingEntry == null)
                        continue;
                    if (rankingEntry.score < minScore)
                    {
                        minScore = rankingEntry.score;
                        minVal = child;
                    }
                }
                // 새 점수가 순위에 들지못했으면, 실패했다고 처리
                if (minScore > score)
                    return TransactionResult.Abort();
                leaders.Remove(minVal);
            }
            // 리스트에 새 점수를 추가
            var newEntry = new RankingInfo(score, nickname, timestamp);
            leaders.Add(newEntry.ToDictionary());
            // 내림차순으로 정렬
            leaders.Sort((a, b) =>
            {
                var rankingA = new RankingInfo(a as Dictionary<string, object>);
                var rankingB = new RankingInfo(b as Dictionary<string, object>);
                return rankingB.score.CompareTo(rankingA.score);
            });
            mutableData.Value = leaders;
            // Ranking UI에 반영
            rankingInfoList.Clear();
            foreach (var v in leaders)
                rankingInfoList.Add(new RankingInfo(v as Dictionary<string, object>));
            // 성공했다고 처리
            return TransactionResult.Success(mutableData);
        });
    }
}