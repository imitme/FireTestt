using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FB0109 : MonoBehaviour
{
    private const string ScoreCollectionName = "Scores";
    private static DatabaseReference ScoresReference;

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

    private void Awake()
    {
        ScoresReference = FirebaseDatabase.DefaultInstance.GetReference(ScoreCollectionName);
    }

    public int count = 0;

    public void OnClicked()
    {
        WriteNewScore(count++, "나", System.DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss"));
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
}