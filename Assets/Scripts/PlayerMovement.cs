using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private GameObject camera;
    [SerializeField] private float speed;
    [SerializeField] private float jumpSpeed;
    private Rigidbody2D body;
    private Rigidbody2D teleBody;
    //private bool grounded;

    private bool tele1;
    private bool tele2;
    private bool tele3;
    private bool tele4;
    
    float defaultSpawnTime = 2.46f; // time between beats * 2
    float spawnTime;
    [SerializeField] private GameObject bg;
    [SerializeField] private GameObject canvas;
    [SerializeField] private GameObject teleport;
    private string cyan = "cyan";
    private string magenta = "magenta";
    private Color def = new Color(0.0f, 0.3541925f, 0.4867924f, 1f);
    private Color defSafe = new Color(0.182079f, 0.4037736f, 0.221417f, 1f);
    private Color red = new Color(0.5320754f, 0.09537195f, 0.1113978f, 1f);
    private Color sphere = new Color(0.5843138f, 0.8666667f, 0.9921569f, 1f);
    private Color fakeSphere = new Color(0.5843138f, 0.8666667f, 0.9921569f, 0f);
    private string temp;
    private bool safe = true;
    private int counter = 5;
    private Vector2 pos;
    private bool locked;
    private bool boost;
    private bool fail;
    private bool tele;
    private bool warped;
    private bool pressingDown;
    private bool swapped;


    private int swap;
    private GameObject redBlock;
    private GameObject blueBlock;
    private GameObject greenBlock;
    private Color redA = new Color(1f, 0.3301886f, 0.3301886f, 1f);
    private Color redB = new Color(1f, 0.3301886f, 0.3301886f, 0.3f);
    private Color blueA = new Color(0.390566f, 0.4451497f, 1f, 1f);
    private Color blueB = new Color(0.390566f, 0.4451497f, 1f, 0.3f);
    private Color greenA = new Color(0.1893556f, 0.7433963f, 0.2781564f, 1f);
    private Color greenB = new Color(0.1893556f, 0.7433963f, 0.2781564f, 0.3f);
    private AudioSource audioSource;
    private float timeSinceSwap = 0f;
    private float firstBeatTime = 0.754f; // seconds before first beat
    private float beatTime = 1.28f; // seconds between beats
    private float prevTime;

    // character and animator
    private GameObject character;
    private Animator charaAnim;

    // raycast grounded check params
    [SerializeField] private Vector2 boxSize;
    [SerializeField] private float castDistance;
    [SerializeField] private LayerMask groundLayer;

    // make "dropVec" a param
    private Vector2 dropVec;

    private void Awake() {
        body = GetComponent<Rigidbody2D>();
        teleBody = teleport.GetComponent<Rigidbody2D>();
        //grounded = true;

        temp = cyan;
        pos = new Vector2(0, 0);
        locked = false;
        boost = false;
        fail = false;
        tele = false;
        warped = false;
        pressingDown = false;
        swapped = false;

        swap = 0;
        redBlock = GameObject.Find("Red");
        blueBlock = GameObject.Find("Blue");
        greenBlock = GameObject.Find("Green");

        teleport.GetComponent<SpriteRenderer>().color = fakeSphere;
        audioSource = GetComponent<AudioSource>();

        // initialize character object and its animator
        character = this.gameObject.transform.GetChild(0).gameObject;
        charaAnim = character.GetComponent<Animator>();
    }

    private void Update() {

        if (Input.GetKeyDown(KeyCode.R)) {
            SceneManager.LoadScene("SampleScene");
        }

        float horizontalInput = Input.GetAxis("Horizontal");
        if (horizontalInput != 0 && !locked) {
            body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);
            
            // animator for moving and correct direction
            charaAnim.SetBool("isRunning", true);
            if (horizontalInput > 0)
                charaAnim.SetFloat("Direction", 0);
            else if (horizontalInput < 0)
                charaAnim.SetFloat("Direction", 1);
        } else {
            body.velocity = new Vector2(0, body.velocity.y);
            
            // animator for idle
            charaAnim.SetBool("isRunning", false);
        }

        if (Input.GetKey(KeyCode.Space)) {
            if (isGrounded() && !locked && !tele) {
                Jump();
            }
        }

        if (Input.GetKeyDown(KeyCode.S)) {
            pressingDown = true;
        }
        if (Input.GetKeyUp(KeyCode.S)) {
            pressingDown = false;
        }

        // When key is pressed, teleport action happens
        if (Input.GetKeyDown(KeyCode.LeftShift)) {
            if (!tele) {
                // shoot teleporter by mouse direction
                var mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                var mouseWorld2D = new Vector2(mouseWorldPos.x, mouseWorldPos.y);
                var diffVec = mouseWorld2D - body.position;

                // set vectors and animator direction by mouse position
                tele = true;
                locked = true;
                Vector2 veloVec = diffVec.normalized * 14.14f;
                if (diffVec.x < 0)
                {
                    dropVec = new Vector2(-1, 1);
                    charaAnim.SetFloat("Direction", 1);
                }
                else if (diffVec.x > 0)
                {
                    dropVec = new Vector2(1, 1);
                    charaAnim.SetFloat("Direction", 0);
                }

                // When pressing down, drop ball next to player.
                if (pressingDown) {
                    dropVec = new Vector2(1, 0);
                    veloVec = new Vector2(0, 0);
                }
            
                // Setup position, color, collider, and rigidbody.
                teleBody.position = body.position + dropVec;
                teleport.GetComponent<SpriteRenderer>().color = sphere;
                teleport.GetComponent<Collider2D>().enabled = true;
                teleBody.constraints = RigidbodyConstraints2D.None;

                teleBody.velocity = veloVec;
                
                // animator for throwing teleporter
                charaAnim.SetTrigger("isThrowing");
            } else {
                teleport.GetComponent<SpriteRenderer>().color = fakeSphere;
                teleport.GetComponent<Collider2D>().enabled = false;
                teleBody.constraints = RigidbodyConstraints2D.FreezePosition;
                body.position = teleBody.position;
                locked = false;
                tele = false;
            }

        }

        timeSinceSwap += audioSource.time - prevTime;
        prevTime = audioSource.time;

        // color change
        Debug.Log(audioSource.time);
        if (timeSinceSwap >= beatTime || audioSource.time == firstBeatTime)
        {
            if (swapped)
            {
                temp = magenta;
                swapBlocks();
                swapped = false;
            } else
            {
                temp = cyan;
                warped = false;
                swapped = true;
            }
            // Change background sprite
            bg.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("farthest-" + temp);
            bg.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("farthest-" + temp);
            bg.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("far-" + temp);
            bg.transform.GetChild(3).gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("far-" + temp);
            bg.transform.GetChild(4).gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("close-" + temp);
            bg.transform.GetChild(5).gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("close-" + temp);
            bg.transform.GetChild(6).gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("closest-" + temp);
            bg.transform.GetChild(7).gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("closest-" + temp);

            timeSinceSwap = 0f;
        }


        // time with the beat
        if (swapped && tele)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("timed");
                teleBody.velocity += new Vector2(4f, 0);
            }
            if (!warped && Input.GetKeyDown(KeyCode.Return))
            {
                teleBody.position += new Vector2(3f, 0);
                warped = true;
            }
        }

        // animator "jump" when not grounded
        if (!isGrounded())
        {
            if (!charaAnim.GetBool("isJumping"))
                charaAnim.SetBool("isJumping", true);
        } else
        {
            charaAnim.SetBool("isJumping", false);
        }
    }

    private void OnTriggerEnter2D(Collider2D col) {
        if (col.gameObject.tag == "DeathPlane") {
            SceneManager.LoadScene("SampleScene");
        }
        // Debug.Log(col.gameObject.transform.position);
    }

    // private void OnTriggerExit2D() {
    //     safe = false;
    //     temp = def;
    //     Debug.Log("Not safe");
    // }

    private void Jump() {
        body.velocity = new Vector2(body.velocity.x, jumpSpeed);
        //grounded = false;
    }

    //private void OnCollisionEnter2D(Collision2D collision) {
    //    if (collision.gameObject.tag == "Ground") {
    //        grounded = true;
    //        charaAnim.SetBool("isJumping", false);
    //    }
    //}

    //private void OnCollisionExit2D(Collision2D collision)
    //{
    //    if (collision.gameObject.tag == "Ground")
    //    {
    //        grounded = false;
    //        charaAnim.SetBool("isJumping", true);
    //    }
    //}

    // check grounded with raycast
    private bool isGrounded()
    {
        if(Physics2D.BoxCast(transform.position, boxSize, 0, -transform.up, castDistance, groundLayer))
        {
            return true;
        } else
        {
            return false;
        }
    }

    // raycast gizmos
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position - transform.up * castDistance, boxSize);
    }

    private void swapBlocks() {
        if (swap == 0) {
            redBlock.GetComponent<BoxCollider2D>().enabled = false;
            blueBlock.GetComponent<BoxCollider2D>().enabled = true;
            greenBlock.GetComponent<BoxCollider2D>().enabled = false;
            greenBlock.GetComponent<SpriteRenderer>().color = greenB;
            blueBlock.GetComponent<SpriteRenderer>().color = blueA;
        } else if (swap == 1) {
            redBlock.GetComponent<BoxCollider2D>().enabled = true;
            blueBlock.GetComponent<BoxCollider2D>().enabled = false;
            greenBlock.GetComponent<BoxCollider2D>().enabled = false;
            blueBlock.GetComponent<SpriteRenderer>().color = blueB;
            redBlock.GetComponent<SpriteRenderer>().color = redA;
        } else {
            redBlock.GetComponent<BoxCollider2D>().enabled = true;
            blueBlock.GetComponent<BoxCollider2D>().enabled = false;
            greenBlock.GetComponent<BoxCollider2D>().enabled = true;
            redBlock.GetComponent<SpriteRenderer>().color = redB;
            greenBlock.GetComponent<SpriteRenderer>().color = greenA;
        }
        swap = (swap + 1) % 3;
    }
}
