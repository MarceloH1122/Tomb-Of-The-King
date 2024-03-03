using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Usually I dont code this way, but I have just 10 days so that's it
// quick and dirty, run baby

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]

public class PlayerMoviment : MonoBehaviour
{
	[Header("Moviment")]
	public float MaxSpeed = 10f;
	public float Acceleration = 7f;
	public float Decceleration = 7f;
	public float AccelerationPower = 1f;
	public float FrictionMultiplier = .2f;
	
	
	[Header("Jump")]
	public ColliderCheck GroundDetector;
	public float JumpForce = 10f;
	public float FallGravityMultiplier = 3.0f;
	[Range(0f, 1f)]
	public float JumpCutMultiplier = 0f;
	
	
	[Header("Roll")]
	public float VelocityMultiplier = 1.3f;
	
	
	private bool _IsMoving = false;
	private bool _IsFlipped = false;
	private bool _IsGrounded = false;
	private bool _RollTrigged = false;
	private bool _JumpTrigged = false;
	private bool _CanJump = false;
	private bool _CanRoll = false;
	private float _MovimentSpeed = .0f;
	
	private float _GravityScale = 0f;
	private float _MaxCoyoteTime = .5f;
	private float _CoyoteTimer = 0f;
	private float _JumpTimer = 0f;
	
	
	// internal things
	private Rigidbody2D rb;
	private Animator an;
	private SpriteRenderer sr;
	
	
	#region Ground Functions
	void CheckGroundOn(Collider2D col)
	{
		if(!col.CompareTag("Ground"))
			return;
		_IsGrounded = true;
		print("earth");
	}
	
	void CheckGroundOff(Collider2D col) {
		if(!col.CompareTag("Ground"))
			return;
		_IsGrounded = false; 
		print("air");
	}
	#endregion
	
	
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
		an = GetComponent<Animator>();
		sr = GetComponent<SpriteRenderer>();
		
		_GravityScale = rb.gravityScale;
		
		GroundDetector.OnEnterCall += CheckGroundOn;
		GroundDetector.OnExitCall += CheckGroundOff;
    }
		

    // Update is called once per frame
    void Update()
    {
		_CoyoteTimer += Time.deltaTime;
		_JumpTimer   += Time.deltaTime;
		
		// I already need this here for animation, sad but ok
        _MovimentSpeed = Input.GetAxis("Horizontal");
		_IsMoving = (Mathf.Abs(_MovimentSpeed) > .1f);
		
		_JumpTrigged = Input.GetKeyDown(KeyCode.W);
		_RollTrigged = Input.GetKeyDown(KeyCode.Space);
		
		if(_IsGrounded)
		{
			_CanJump = true;
			_CanRoll = true;
			_CoyoteTimer = 0;
		}
		if(_CoyoteTimer <= _MaxCoyoteTime && _JumpTimer > _CoyoteTimer)
		{
			_CanJump = true;
		}
		
		
		if(_IsMoving){
			if(_MovimentSpeed > 0f)
				sr.flipX = true;
			else
				sr.flipX = false;
		}
		
		
		
		
		// ? should I use id here?
		an.SetFloat("VerticalVelocity", rb.velocity.y);
		an.SetBool("IsGrounded", _IsGrounded);
		an.SetBool("IsMoving", _IsMoving);
		if(_JumpTrigged && _CanJump)
		{
			an.SetTrigger("Jump");
			Jump();
			_JumpTimer = 0;
		}
		if(_RollTrigged && _CanRoll)
		{
			an.SetTrigger("Roll");
			Roll();
		}	
    }
	
	private void Jump()
	{
		rb.AddForce(JumpForce * Vector2.up, ForceMode2D.Impulse);
	}
	private void Roll()
	{
		
	}
	
	private void CheckRoll(){
	}
	
	
	void PostUpdate(){
		_RollTrigged = false;
		_JumpTrigged = false;
		_CanJump = false;
		_CanRoll = false;
	}
	
	
	void FixedUpdate(){
		#region Moviment
		float targetSpeed = _MovimentSpeed * MaxSpeed;
		float speedDiff = targetSpeed - rb.velocity.x;
		
		// ace rate, 72%. aro rate 63%
		float acceRate = (Mathf.Abs(targetSpeed) > .01f) ? Acceleration : Decceleration;
		float mov = Mathf.Pow(Mathf.Abs(speedDiff) * acceRate, AccelerationPower) * Mathf.Sign(speedDiff);
		
		rb.AddForce(mov * Vector2.right);
		#endregion
		
		#region stop
		if(!_IsMoving && _IsGrounded)
		{
			float amount = Mathf.Min(Mathf.Abs(rb.velocity.x), Mathf.Abs(FrictionMultiplier));
			amount *= Mathf.Sign(rb.velocity.x);
			rb.AddForce(-amount * Vector2.right, ForceMode2D.Impulse);
		}
		#endregion
		
		if(rb.velocity.y < 0)
		{
			rb.gravityScale = _GravityScale * FallGravityMultiplier;			
		}
		else
		{
			rb.gravityScale = _GravityScale;
		}
	}
}
