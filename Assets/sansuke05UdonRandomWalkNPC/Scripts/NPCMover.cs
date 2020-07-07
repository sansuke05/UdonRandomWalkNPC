
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class NPCMover : UdonSharpBehaviour
{
    private Animator _animator;

    private Vector3 _startPosition;

    private Vector3 _beforePosition;    //  一つ前の座標

    private int _selectedValue;

    /// <summary>
    /// x, zの値を反転させるかの設定値
    /// </summary>
    private int _reversSetting;

    /// <summary>
    /// 目的地
    /// </summary>
    private Vector3 _destination;

    private float _elapsedTime;

    private Vector3 _velocity;

    private Vector3 _direction;

    private bool _arrived;

    private bool _returnPosition;

    private bool _valuesSetted = false;

    public int[] _array = new int[8] { 1, 2, 3, 4, 5, 6, 7, 8 };

    public float _WalkSpeed = 1.0f;

    public float _WaitTime = 5f;


    void Start()
    {
        _startPosition = transform.position;
        _beforePosition = _startPosition;
        _animator = GetComponent<Animator>();
        _velocity = Vector3.zero;
        _arrived = true;
    }


    void Update()
    {
        if (!_arrived)
        {
            Move();

            // 目的地に到着したか
            var currentPosition = transform.position;
            if (Vector3.Distance(currentPosition, _destination) < 0.5)
            {
                _arrived = true;
                _beforePosition = currentPosition;  // 現在の位置を記録
                SetStopAnimation();
            }
        }
        else // 到着していた場合
        {
            _elapsedTime += Time.deltaTime;

            //　待ち時間を越えたら次の目的地を設定
            if (_elapsedTime > _WaitTime)
            {
                // オーナーは値を生成し、それ以外はオーナーの値が配信されるまで待つ
                if (Networking.IsOwner(Networking.LocalPlayer, gameObject))
                {
                    CreateRandomValue();
                    SelectReversValueOrNot();
                    //finishValuesSetting();
                    SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(finishValuesSetting));
                }

                if (_valuesSetted)
                {
                    SetDestination();
                    _arrived = false;
                    _elapsedTime = 0f;
                    _valuesSetted = false;
                }
            }
        }
    }


    private void Move()
    {
        if (Physics.CheckSphere(transform.position, 0.1f)) //接地判定
        {
            _velocity = Vector3.zero;
            SetWalkAnimation();
            _direction = (_destination - transform.position).normalized;
            transform.LookAt(new Vector3(_destination.x, transform.position.y, _destination.z));
            _velocity = _direction * _WalkSpeed;
            //Debug.Log(_Destination);
        }
        _velocity.y += Physics.gravity.y * Time.deltaTime;
        transform.localPosition += _velocity * Time.deltaTime;
    }


    /// <summary>
    /// 次の目的地を設定
    /// </summary>
    private void SetDestination()
    {
        if (_returnPosition)    // 壁にぶつかったら前の位置に戻る
        {
            _destination = _beforePosition;
            _returnPosition = false;
        }
        else
        {
            // 乱数値を使って配列からランダムに値を取り出し、次の目的地として設定
            var vector = _array[_selectedValue];
            switch (_reversSetting) // x, zの値を反転させるか
            {
                // 0 -> (x, 0, y)
                // 1 -> (x, 0, -y)
                // 2 -> (-x, 0, y)
                // 3 -> (-x, 0, -y)
                case 0:
                    _destination = _startPosition + new Vector3(vector, 0, vector);
                    break;
                case 1:
                    _destination = _startPosition + new Vector3(vector, 0, -vector);
                    break;
                case 2:
                    _destination = _startPosition + new Vector3(-vector, 0, vector);
                    break;
                case 3:
                    _destination = _startPosition + new Vector3(-vector, 0, -vector);
                    break;
                default:
                    break;
            }

            Debug.Log("Next Local Destination: " + _destination);
        }
    }


    /// <summary>
    /// 値が生成、配信されたことを通知する
    /// </summary>
    public void finishValuesSetting()
    {
        _valuesSetted = true;
    }


    /// <summary>
    /// オーナーのみ乱数を生成させ、値だけ同期
    /// </summary>
    private void CreateRandomValue()
    {
        var randValue = Random.Range(0, _array.Length);
        // ランダムに選ばれた値を同期させる
        switch (randValue)
        {
            case 0:
                //SyncZeroValue();
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(SyncZeroValue));
                break;
            case 1:
                //SyncOneValue();
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(SyncOneValue));
                break;
            case 2:
                //SyncTwoValue();
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(SyncTwoValue));
                break;
            case 3:
                //SyncThreeValue();
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(SyncThreeValue));
                break;
            case 4:
                //SyncFourValue();
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(SyncFourValue));
                break;
            case 5:
                //SyncFiveValue();
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(SyncFiveValue));
                break;
            case 6:
                //SyncSixValue();
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(SyncSixValue));
                break;
            case 7:
                //SyncSevenValue();
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(SyncSevenValue));
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// x,zの値を反転させるかをランダムに設定する(オーナーのみ)
    /// </summary>
    private void SelectReversValueOrNot()
    {
        var randValue = Random.Range(0, 4);
        // 0 -> (x, 0, y)
        // 1 -> (x, 0, -y)
        // 2 -> (-x, 0, y)
        // 3 -> (-x, 0, -y)
        switch (randValue)
        {
            case 0:
                //SyncZeroSetting();
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(SyncZeroSetting));
                break;
            case 1:
                //SyncOneSetting();
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(SyncOneSetting));
                break;
            case 2:
                //SyncTwoSetting();
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(SyncTwoSetting));
                break;
            case 3:
                //SyncThreeSetting();
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(SyncThreeSetting));
                break;
            default:
                break;
        }
    }


    #region 以下値設定用メソッド

    public void SyncZeroValue()
    {
        _selectedValue = 0;
    }


    public void SyncOneValue()
    {
        _selectedValue = 1;
    }


    public void SyncTwoValue()
    {
        _selectedValue = 2;
    }


    public void SyncThreeValue()
    {
        _selectedValue = 3;
    }


    public void SyncFourValue()
    {
        _selectedValue = 4;
    }


    public void SyncFiveValue()
    {
        _selectedValue = 5;
    }


    public void SyncSixValue()
    {
        _selectedValue = 6;
    }


    public void SyncSevenValue()
    {
        _selectedValue = 7;
    }
    #endregion


    #region 以下値反転設定用メソッド

    public void SyncZeroSetting()
    {
        _reversSetting = 0;
    }


    public void SyncOneSetting()
    {
        _reversSetting = 1;
    }


    public void SyncTwoSetting()
    {
        _reversSetting = 2;
    }


    public void SyncThreeSetting()
    {
        _reversSetting = 3;
    }
    #endregion

    #region 以下アニメーション用メソッド

    public void SetStopAnimation()
    {
        _animator.SetFloat("Speed", 0.0f);
    }


    public void SetWalkAnimation()
    {
        _animator.SetFloat("Speed", 2.0f);
    }
    #endregion


    // コライダー判定
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("EnterCollision");
        if (collision.gameObject.layer == 23) // 23:NPCCollider
        {
            Debug.Log("Arrived");
            _arrived = true;
            _returnPosition = true;         // はまり防止
            SetStopAnimation();
        }
    }
}
