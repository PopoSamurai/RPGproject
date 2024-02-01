using UnityEngine;

public class Table : MonoBehaviour
{
    Vector3 diceDir;
    void FixedUpdate()
    {
        diceDir = Dice.direction;
    }

    void OnTriggerEnter(Collider coll)
    {
        if(diceDir.x == 0f && diceDir.y == 0f && diceDir.z == 0f)
        {
            switch (coll.gameObject.name)
            {
                //œciany z naprzeciwka i -1 gdy¿ lista zaczyna siê od ele 0
                case "Side1":
                    GameM.number = 7;
                    break;
                case "Side2":
                    GameM.number = 6;
                    break;
                case "Side3":
                    GameM.number = 8;
                    break;
                case "Side4":
                    GameM.number = 10;
                    break;
                case "Side5":
                    GameM.number = 9;
                    break;
                case "Side6":
                    GameM.number = 11;
                    break;
                case "Side7":
                    GameM.number = 1;
                    break;
                case "Side8":
                    GameM.number = 0;
                    break;
                case "Side9":
                    GameM.number = 2;
                    break;
                case "Side10":
                    GameM.number = 4;
                    break;
                case "Side11":
                    GameM.number = 3;
                    break;
                case "Side12":
                    GameM.number = 5;
                    break;
            }
        }
    }
}
