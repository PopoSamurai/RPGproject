using BattleSystem;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MapChange : MonoBehaviour
{
    public Transform[] points;
    public Teammates team;

    void Start()
    {
        if (team.allCharacters[0] != null)
        {
            //main
        }
        if (team.allCharacters[1] != null)
        {
            GameObject follower1 = Instantiate(team.allCharacters[1].GetComponent<CharacterClass>().teamPref, points[0].position, points[0].rotation);
            follower1.GetComponent<FollowTeam>().stopDistance = 1.3f;
        }
        if (team.allCharacters[2] != null)
        {
            GameObject follower2 = Instantiate(team.allCharacters[2].GetComponent<CharacterClass>().teamPref, points[1].position, points[1].rotation);
            follower2.GetComponent<FollowTeam>().stopDistance = 2.8f;
        }
        if (team.allCharacters[3] != null)
        {
            GameObject follower3 = Instantiate(team.allCharacters[3].GetComponent<CharacterClass>().teamPref, points[2].position, points[2].rotation);
            follower3.GetComponent<FollowTeam>().stopDistance = 4.3f;
        }
    }
}
