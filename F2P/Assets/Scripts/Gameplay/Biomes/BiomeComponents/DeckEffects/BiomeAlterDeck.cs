using Com.IsartDigital.F2P.Biomes.Effects;

using Unity.VisualScripting;
using UnityEngine;

// Author (CR) : Lefevre Florian
namespace Com.IsartDigital.F2P.Biomes
{
    public class BiomeAlterDeck : MonoBehaviour
    {
        [Header("Design")]
        [SerializeField][Min(1)] private int _NbAffected = 1;

        [Space(2)]
        [SerializeField] private DeckEffect.AlterationType _Type = default;
        [SerializeField] private bool _DestroyEffectWithInstance = false;

        // Variables
        private int _NbTurn = 1;

        public void ContinuousAlteration(int pNbTurn = 1)
        {
            _NbTurn = pNbTurn;
            SetupDeckEffect();
        }

        public void ImmediateAlteration()
        {
            if (_Type == DeckEffect.AlterationType.Positive)
                HandManager.GetInstance().AddCardToDeck(_NbAffected);
            else
                HandManager.GetInstance().BurnCard(_NbAffected);

            HandManager.OnDeckAltered.Invoke(_NbAffected, GetComponent<Biome>().Type);
        }

        public void ImmmediateAlteration(MonoBehaviour pBonus)
        {
            if (pBonus is IBiomeEnumerator)
            {
                _NbAffected = (pBonus as IBiomeEnumerator).GetEnumertation();
                ImmediateAlteration();
            }
            else
                Debug.LogError("Must be an" + typeof(IBiomeEnumerator));
        }

        private void SetupDeckEffect()
        {
            DeckEffect lEffect = null;
            if (_DestroyEffectWithInstance)
            {
                lEffect = gameObject.AddComponent<DeckEffect>();
                lEffect.SetEffect(_NbTurn,
                                  _NbAffected,
                                  _Type);

                HandManager.OnDeckAltered.Invoke(_NbAffected * _NbTurn, GetComponent<Biome>().Type);
            }
            else
            {
                Player lPlayer = Player.GetInstance();
                if(lPlayer.GetComponent<DeckEffect>() == null)
                {
                    lEffect = lPlayer.AddComponent<DeckEffect>();

                    lEffect.SetEffect(_NbTurn,
                                      _NbAffected,
                                      _Type);

                    HandManager.OnDeckAltered.Invoke(_NbAffected * _NbTurn, GetComponent<Biome>().Type);
                }
                else
                {
                    lPlayer.GetComponent<DeckEffect>().IncrementTimer();
                    HandManager.OnDeckAltered.Invoke(1, GetComponent<Biome>().Type);
                }
                    
            }
        }
    }
}
