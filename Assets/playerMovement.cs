using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMovement : MonoBehaviour
{
    public CharacterController2D controller2D;
    public jumpPlatforms effector;
    public Animator animator;
    public Frog frog;
    public Frog frog2;
    public bird Bird;
    public opossum Opossum;
    public Gem gem;
    public Gem gem2;
    public Gem gem3;
    public Gem gem4;
    public Gem gem5;
    public Gem gem6;
    public Gem gem7;
    public Cherry cherry;
    public Cherry cherry2;
    public Cherry cherry3;
    public Cherry cherry4;
    public Cherry cherry5;
    public Cherry cherry6;
    public float runSpeed = 40f;
    float horizontalMove = 0f;
    bool jump = false;
    bool crouch = false;
    bool hurt = false;

    private void Start() {
  }
    // Update is called once per frame
    void Update() {
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;

        animator.SetFloat("Speed", Mathf.Abs(horizontalMove));

        if (Input.GetButtonDown("Jump")) {
            jump = true;
        }

        if (Input.GetButtonDown("Crouch")) {
            crouch = true;
            effector.EffectorRotation(180);
        } else if (Input.GetButtonUp("Crouch")) {
            crouch = false;
            effector.EffectorRotation(0);
        }
    }

    public void OnLand() {
        animator.SetBool("IsJumping", false);
        animator.SetBool("IsFalling", false);
        animator.SetBool("isHurt", false);
    }

    public void WhileRising(bool isRising) {
        animator.SetBool("IsJumping", isRising);
    }
    public void WhileFalling(bool isFalling) {
        animator.SetBool("IsFalling", isFalling);
    }

    public void OnCrouching(bool onCrouching) {
        animator.SetBool("IsCrouching", onCrouching);
    }

    public void OnHurt() {
        animator.SetBool("isHurt", true);
    }

    public void OnEnemyDeath(string name) {
        switch(name) {
            case "Frog": 
                frog.OnTrampoline();
                break;
            case "Frog (1)":
                frog2.OnTrampoline();
                break;
            case "Bird":
                Bird.OnTrampoline();
                break;
            case "opossum":
                Opossum.OnTrampoline();
                break;
        }
    }

    public void OnCollectible(string name) {
        switch(name) {
      case "Gem":
        gem.OnTouch();
        break;
      case "Gem (1)":
        gem2.OnTouch();
        break;
      case "Gem (2)":
        gem3.OnTouch();
        break;
      case "Gem (3)":
        gem4.OnTouch();
        break;
      case "Gem (4)":
        gem5.OnTouch();
        break;
      case "Gem (5)":
        gem6.OnTouch();
        break;
      case "Gem (6)":
        gem7.OnTouch();
        break;
      case "Cherry":
        cherry.OnTouch();
        break;
      case "Cherry (1)":
        cherry2.OnTouch();
        break;
      case "Cherry (2)":
        cherry3.OnTouch();
        break;
      case "Cherry (3)":
        cherry4.OnTouch();
        break;
      case "Cherry (4)":
        cherry5.OnTouch();
        break;
      case "Cherry (5)":
        cherry6.OnTouch();
        break;
        }
    }

    void FixedUpdate() {
        controller2D.Move(horizontalMove * Time.fixedDeltaTime, crouch, jump, hurt);
        jump = false;
    }
}
