using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class Player : MovingObject {

    public Text foodText;
    public int wallDamage = 1;
    public int pointsPerFood = 10;
    public int pointsPerSoda = 20;
    public float restartlevelDelay = 1f;

    private Animator animator;
    private int food;

    protected override void Start()
    {
        animator = GetComponent<Animator>();

        food = GameManager.instance.playerfoodPoints;

        foodText.text = "Food " + food;

        base.Start();
    }

    private void OnDisable()
    {
        GameManager.instance.playerfoodPoints = food;
    }

    void Update () {
        if (!GameManager.instance.playersTurn) return;

        int horizontal = 0;
        int vertical = 0;

        horizontal = (int)Input.GetAxisRaw("Horizontal");
        vertical = (int)Input.GetAxisRaw("Vertical");

        if (horizontal != 0)
            vertical = 0;

        if (horizontal != 0 || vertical != 0)
            AttemptMove<Wall>(horizontal, vertical);
	}


    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        food--;
        foodText.text = "Food " + food;

        base.AttemptMove<T>(xDir, yDir);
        RaycastHit2D hit;

        CheckIfGameOver();

        GameManager.instance.playersTurn = false;
    }

    // Normal Trigger !!!!!!!!!
    private void OnTriggerEnter2D(Collider2D o)
    {
        if (o.tag == "Exit")
        {
            Invoke("Restart", restartlevelDelay);
            enabled = false;
        } else if (o.tag == "Food") {
            food += pointsPerFood;
            foodText.text = "+" + pointsPerFood + " Food:" + food;
            o.gameObject.SetActive(false);
        } else if (o.tag == "Soda") {
            food += pointsPerSoda;
            foodText.text = "+" + pointsPerSoda + " Food:" + food;
            o.gameObject.SetActive(false);
        }
    }


    protected override void OnCantMove<T>(T component)
    {
        Wall hitwall = component as Wall;
        hitwall.DamageWall(wallDamage);
        animator.SetTrigger("playerChop");
    }

    private void Restart()
    {
        SceneManager.LoadScene(0);
    }

    public void LoseFood (int loss)
    {
        animator.SetTrigger("playerHit");
        food -= loss;
        foodText.text = "-" + loss + " Food:" + food;
        CheckIfGameOver();
    }

    private void CheckIfGameOver()
    {
        if (food <= 0) {
            GameManager.instance.GameOver();
        }
    }
}
