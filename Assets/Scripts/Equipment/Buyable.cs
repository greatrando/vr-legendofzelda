using UnityEngine;


public class Buyable : MonoBehaviour
{


    private const string PLAYER_GAMEOBJECT_NAME = "XR Rig";


    public int Cost = 0;
    public AudioSource PurchasedAudio = null;


    private bool _purchased = false;

     
    void OnCollisionEnter(Collision col) 
    {
        if (!col.gameObject.IsChildOf(PLAYER_GAMEOBJECT_NAME) || _purchased) return;

        // DebugHUD.GetInstance().PresentToast(col.gameObject.name);

        Player player = Player.GetInstance();
        if (player.Wallet.CurrentValue >= Cost)
        {
            _purchased = true;
            player.Wallet.CurrentValue -= Cost;

            if (PurchasedAudio != null)
            {
                player.PlayAudio(PurchasedAudio);
            }

            Destroy(this.gameObject);
        }
    }


}
