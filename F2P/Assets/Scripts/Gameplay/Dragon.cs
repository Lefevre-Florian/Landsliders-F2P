using System;
using System.Collections;
using com.isartdigital.f2p.gameplay.manager;
using UnityEngine;

// Author (CR): Dorian Husson
namespace Com.IsartDigital.F2P.Gameplay
{
    public enum Direction { Up = 0, Down = 1, Right = 2, Left = 3 }
    public class Dragon : MonoBehaviour
    {
        private Direction[] _Directions;
        private int _RandomIndex;
        private GridManager _GridManager;
        private GameManager _GameManager;
        private HandManager _HandManager;
        private Player _Player;


        private Vector2 _NextDirection;
        private Vector2 _ToPosition;
        private Quaternion _ToRotation;

        private bool _CardBurnt;
        [SerializeField] int _NbCardsBurnt = 4;

        private bool _IsDone;

        public bool IsDone { get { return _IsDone; } }

        private bool _CanMove;
        private float _Speed = 5f;
        private float _RotateSpeed = 100f;

        private Action _DoAction;

        private void Start()
        {
            _GridManager = GridManager.GetInstance();
            _GameManager = GameManager.GetInstance();
            _HandManager = HandManager.GetInstance();
            _Player = Player.GetInstance();

            _GameManager.randomEventObjects.Add(this);
            _Directions = (Direction[])System.Enum.GetValues(typeof(Direction));
            _GameManager.OnAllEffectPlayed += DragonMove;
            CheckDirection();
            transform.rotation = _ToRotation;
            SetModeVoid();
        }

        private void Update()
        {
            _DoAction();

            if(_CanMove)
            {
                SetModeMove();
            }

            if ((Vector2)transform.position == (Vector2)_Player.transform.position && !_CardBurnt && !_Player.isProtected)
            {
                OnPlayerPosition();
            }
        }

        private void SetModeVoid()
        {
            _DoAction = DoActionVoid;
        }

        private void DoActionVoid()
        {

        }

        private void SetModeMove()
        {
            _DoAction = DoActionMove;
        }

        private void DoActionMove()
        {
            float t = _Speed * Time.deltaTime;

            transform.position = Vector3.MoveTowards(transform.position, _GridManager.GetWorldCoordinate((int)_ToPosition.x, (int)_ToPosition.y), t);

            if (transform.position == (Vector3)_GridManager.GetWorldCoordinate((int)_ToPosition.x, (int)_ToPosition.y))
            {
                CheckDirection();
                _CanMove = false;
                SetModeRotate();
            }
        }

        private void SetModeRotate()
        {
            _DoAction = DoActionRotate;
        }

        private void DoActionRotate()
        {
            float t = _RotateSpeed * Time.deltaTime;

            transform.rotation = Quaternion.RotateTowards(transform.rotation, _ToRotation, t);

            if (transform.rotation == _ToRotation)
            {
                _IsDone = true;
                SetModeVoid();
            }
        }

        private void CheckDirection()
        {
            _NextDirection = RandomDirection(GetRandomDirection());
            _ToPosition = _GridManager.GetGridCoordinate(transform.position) + _NextDirection;

            for (int i = 0; i < 3; i++)
            {

                if (_ToPosition.x >= 0f  && _ToPosition.x < _GridManager._NumCard.x && _ToPosition.y >= 0f && _ToPosition.y < _GridManager._NumCard.y)
                {
                    break;
                }
                else
                {
                    _NextDirection = Quaternion.AngleAxis(90,Vector3.forward) * _NextDirection;

                    _NextDirection.Normalize();
                    _NextDirection.x = Mathf.RoundToInt(_NextDirection.x);
                    _NextDirection.y = Mathf.RoundToInt(_NextDirection.y);
                    _ToPosition = _GridManager.GetGridCoordinate(transform.position) + _NextDirection;
                }
            }

            _ToRotation = Quaternion.LookRotation(Vector3.forward, (Vector3)_GridManager.GetWorldCoordinate((int)_ToPosition.x, (int)_ToPosition.y) - new Vector3(transform.position.x, transform.position.y, 0));
        }

        private Vector2 RandomDirection(Direction pDirection)
        {
            switch (pDirection)
            {
                case Direction.Up:
                    return Vector2.up;
                case Direction.Down:
                    return Vector2.down;
                case Direction.Right:
                    return Vector2.right;
                case Direction.Left:
                    return Vector2.left;
                default:
                    return Vector2.zero;
            }
        }

        private Direction GetRandomDirection()
        {
            _RandomIndex = UnityEngine.Random.Range(0, _Directions.Length);

            return _Directions[_RandomIndex];
        }

        private void DragonMove()
        {
            _CanMove = true;
            _IsDone = false;
            _CardBurnt = false;
        }

        private void OnPlayerPosition()
        {
            _HandManager.BurnCard(_NbCardsBurnt);
            _CardBurnt = true;
        }

        private void OnDestroy()
        {
            _GameManager.randomEventObjects.Remove(this);
        }
    }
}
