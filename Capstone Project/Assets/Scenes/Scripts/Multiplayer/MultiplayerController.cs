using UnityEngine;

public class MultiplayerController : MonoBehaviour
{
    public float playerSpeed;
    public Rigidbody rb;
    public SpriteRenderer sr;
    private Alteruna.Avatar _avatar;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _avatar = GetComponent<Alteruna.Avatar>();

        if (!_avatar.IsMe)
            return;

        rb = GetComponent<Rigidbody>();

    }

    // Update is called once per frame
    void Update()
    {
        if (!_avatar.IsMe)
            return;

        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        Vector3 moveDir = new Vector3(x, 0, y * 2);

        //Movement code
        rb.linearVelocity = moveDir * playerSpeed;

        //Flip if sprite moves in an opposite direction
        if (x != 0 && x < 0)
        {
            sr.flipX = true;
        }
        else if (x != 0 && x > 0)
        {
            sr.flipX = false;
        }
    }
}
