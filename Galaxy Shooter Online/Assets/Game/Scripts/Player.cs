using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public bool canTripleShot = false;

    public bool speedPowerUp = false;

    public bool shieldActive = false;

    public int lives = 3;


    [SerializeField]
    private GameObject _shieldGameObject;
    [SerializeField]
    private GameObject _explosionPrefab;

    [SerializeField]

    private GameObject _laserPrefab;

    [SerializeField]

    private GameObject _tripleShotPrefab;

    [SerializeField]

    private float _fireRate = 0.25f;
   
    private float _canFire = 0.0f;

    [SerializeField]
    private float _speed = 5.0f;

    private UIManager _uiManager;

    private GameManager _gameManager;
    private SpawnManager _spawnManager;

    private AudioSource _audioSource;
    [SerializeField]
    private GameObject[] _engines;

    private int _hitCount = 0;

	// Use this for initialization
	void Start () {
        transform.position = new Vector3(0, 0, 0);

        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();

        if(_uiManager != null)
        {
            _uiManager.UpdateLives(lives);
        }

        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();




        if (_spawnManager != null)
        {

            _spawnManager.StartSpawnRoutines();
        }

        _audioSource = GetComponent<AudioSource>();

        _hitCount = 0;

    }
	
	// Update is called once per frame
	void Update () {

        Movement();

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButton(0) )
        {
            Shoot();
           
        }
        }
        private void Shoot()
        {
        if (Time.time > _canFire)
        {
            _audioSource.Play();
            if(canTripleShot == true)
            {
                Instantiate(_tripleShotPrefab, transform.position, Quaternion.identity);
            }
            else
            {
                Instantiate(_laserPrefab, transform.position + new Vector3(0, 1, 0), Quaternion.identity);
            }
                     
            _canFire = Time.time + _fireRate;
        }
    }
        
	
    public void Movement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        if (speedPowerUp == true)
        {
           
            transform.Translate(Vector3.right * _speed *1.5f* horizontalInput * Time.deltaTime);
           
            transform.Translate(Vector3.up * _speed *1.5f* verticalInput * Time.deltaTime);
        }
        else 
        {
           
            transform.Translate(Vector3.right * _speed * horizontalInput * Time.deltaTime);

            transform.Translate(Vector3.up * _speed * verticalInput * Time.deltaTime);
        }

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            // Get movement of the finger since last frame
            Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;

            // Move object across XY plane
            transform.Translate(-touchDeltaPosition.x * _speed, -touchDeltaPosition.y * _speed, 0);
        }




        if (transform.position.y > 0)
        {
            transform.position = new Vector3(transform.position.x, 0, 0);
        }
        else if (transform.position.y < -4.2f)
        {
            transform.position = new Vector3(transform.position.x, -4.2f, 0);
        }

        if (transform.position.x > 8.2f)
        {
            transform.position = new Vector3(8.2f, transform.position.y, 0);
        }
        else if (transform.position.x < -8.2f)
        {
            transform.position = new Vector3(-8.2f, transform.position.y, 0);
        }
    }

    public void Damage()
    {
        //subtracts 1 life from player
        //if the player has shield
        //do nothing

       
        if(shieldActive == true)
        {
            shieldActive = false;

            _shieldGameObject.SetActive(false);
            return;
        }
        _hitCount++;

        if (_hitCount == 1)
        {
            _engines[0].SetActive(true);
        }
        else if (_hitCount == 2)
        {
            _engines[1].SetActive(true);
        }


        lives--;
        _uiManager.UpdateLives(lives);

        if(lives < 1)
        {
            Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
            _gameManager.gameOver = true;
            _uiManager.ShowTitleScreen();
            Destroy(this.gameObject);
        }
    }
    public void TripleShotPowerupon()
    {
        canTripleShot = true;
        StartCoroutine(TripleShotPowerDownRoutine());
    }

    public void SpeedPowerUpRoutine()
    {
        speedPowerUp = true;
        StartCoroutine(SpeedPowerDownRoutine());
    }

    public void EnableShields()
    {
        shieldActive = true;

        _shieldGameObject.SetActive(true);
    }

    public IEnumerator SpeedPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        speedPowerUp = false;
    }
   public IEnumerator TripleShotPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        canTripleShot = false;
    }
}
