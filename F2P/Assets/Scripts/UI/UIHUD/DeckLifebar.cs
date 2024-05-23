using Com.IsartDigital.F2P.Biomes;
using System.Collections;
using TMPro;

using UnityEngine;

// Author (CR) : Lefevre Florian
namespace Com.IsartDigital.F2P.UI.UIHUD
{
    public class DeckLifebar : MonoBehaviour
    {
        [Header("UI Utils")]
        [SerializeField] private TextMeshProUGUI _CounterLabel = null;
        [SerializeField] private RectTransform _Counter = null;

        [Space(5)]
        [SerializeField][Range(0.1f, 1.0f)] private float _Duration = 1f;

        // Variables
        private HandManager _HandManager = null;

        private Vector3 _InitialPosition, _AlteredPosition = default;

        private Coroutine _Coroutine = null;

        private void Start()
        {
            _HandManager = HandManager.GetInstance();

            GameManager.CardPlaced.AddListener(UpdateHealth);
            HandManager.OnDeckAltered.AddListener(WrapperUpdateHealth);

            _InitialPosition = _Counter.transform.localPosition;
            _AlteredPosition = _InitialPosition + Vector3.down * (_Counter.rect.height / 2f);

            // Phase flow
            GameManager.CardPlaced.AddListener(Display);
            GameManager.GetInstance().OnTurnPassed += Hide;

            _Counter.localPosition = _AlteredPosition;
        }

        private IEnumerator Animation(Vector3 pSource, Vector3 pFinale)
        {
            float lSpeed = 1f / _Duration;
            float lElapsedTime = 0f;

            _Counter.localPosition = pSource;

            while (lElapsedTime < 1f)
            {
                lElapsedTime += Time.deltaTime * lSpeed;
                _Counter.localPosition = Vector3.Lerp(pSource, pFinale, lElapsedTime);

                yield return new WaitForEndOfFrame();
            }

            _Counter.localPosition = pFinale;

            if (_Coroutine != null)
                StopCoroutine(_Coroutine);
            _Coroutine = null;
        }

        public void UpdateHealth() => _CounterLabel.text = _HandManager._TotalCards.ToString();

        private void WrapperUpdateHealth(int pCount, BiomeType pType) => UpdateHealth();

        private void Display() => _Coroutine = StartCoroutine(Animation(_AlteredPosition, _InitialPosition));

        private void Hide() => _Coroutine = StartCoroutine(Animation(_InitialPosition, _AlteredPosition));

        private void OnDestroy()
        {
            GameManager.CardPlaced.RemoveListener(UpdateHealth);
            HandManager.OnDeckAltered.RemoveListener(WrapperUpdateHealth);

            GameManager.CardPlaced.RemoveListener(Display);
            GameManager.GetInstance().OnTurnPassed -= Hide;

            _HandManager = null;

            if (_Coroutine != null)
                StopCoroutine(_Coroutine);
        }
    }
}
