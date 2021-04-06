using UnityEngine;


public class Buyable : MonoBehaviour
{


    private const string PLAYER_GAMEOBJECT_NAME = "XR Rig";


    public int Cost = 0;

     
    void OnCollisionEnter(Collision col) 
    {
        if (!col.gameObject.IsChildOf(PLAYER_GAMEOBJECT_NAME)) return;

        DebugHUD.GetInstance().PresentToast(col.gameObject.name);

        Player player = Player.GetInstance();
        if (player.Wallet.CurrentValue >= Cost)
        {
            player.Wallet.CurrentValue -= Cost;

            Destroy(this.gameObject);
        }
    }


}
