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
    private bool grounded;

    private bool tele1;
    private bool tele2;
    private bool tele3;
    private bool tele4;

    float defaultSpawnTime = 3f;
    float spawnTime = 3f;
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

    private void Awake() {

        body = GetComponent<Rigidbody2D>();
        teleBody = teleport.GetComponent<Rigidbody2D>();
        grounded = true;

        temp = cyan;
        //obj.GetComponent<SpriteRenderer>().color = def; //unneeded with new background
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

    }

    private void Update() {

        if (Input.GetKeyDown(KeyCode.R)) {
            SceneManager.LoadScene("SampleScene");
        }

        float horizontalInput = Input.GetAxis("Horizontal");
        if (horizontalInput != 0 && !locked) {
            body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);
        } else {
            body.velocity = new Vector2(0, body.velocity.y);
        }

        if (Input.GetKey(KeyCode.Space)) {
            if (grounded && !locked && !tele) {
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
                tele = true;
                locked = true;
                Vector2 dropVec = new Vector2(1, 1);
                Vector2 veloVec = new Vector2(10, 10);

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

            } else {
                teleport.GetComponent<SpriteRenderer>().color = fakeSphere;
                teleport.GetComponent<Collider2D>().enabled = false;
                teleBody.constraints = RigidbodyConstraints2D.FreezePosition;
                body.position = teleBody.position;
                locked = false;
                tele = false;
            }

        }

        spawnTime -= Time.deltaTime;

        //bg.GetComponent<SpriteRenderer>().color = temp;
        // Change background sprite with the rhythm
        bg.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("farthest-" + temp);
        bg.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("farthest-" + temp);
        bg.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("far-" + temp);
        bg.transform.GetChild(3).gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("far-" + temp);
        bg.transform.GetChild(4).gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("close-" + temp);
        bg.transform.GetChild(5).gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("close-" + temp);
        bg.transform.GetChild(6).gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("closest-" + temp);
        bg.transform.GetChild(7).gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("closest-" + temp);
        
        if ((spawnTime < 0.7f && spawnTime < 0.8f) || (spawnTime > 2.3f && spawnTime < 2.2f)) {
            temp = magenta;
            if (!swapped) {
                swapBlocks();
                swapped = true;
            }
            
            if (tele) {
                if (Input.GetKeyDown(KeyCode.Space)) {
                    Debug.Log("timed");
                    teleBody.velocity += new Vector2(4f, 0);
                }
                if (!warped && Input.GetKeyDown(KeyCode.Return)) {
                    teleBody.position += new Vector2(3f, 0);
                    warped = true;
                }
            }
        } else {
            warped = false;
            temp = cyan;
            swapped = false;
        }

        if (spawnTime < 0) {
            // swapBlocks();
            // obj.GetComponent<SpriteRenderer>().color = red;
            spawnTime = defaultSpawnTime;
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
        grounded = false;
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.tag == "Ground") {
            grounded = true;
        }
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
