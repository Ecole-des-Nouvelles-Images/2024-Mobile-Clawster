using UnityEngine;
using UnityEngine.AI;

public class NavmeshPlayer : MonoBehaviour
{
    private NavMeshAgent _navMeshAgent;
    private Animator _animator;

    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        if (input.magnitude <=0)
        {
            _animator.SetBool("Walk", false);
            return;
        }

        if (Mathf.Abs(input.y) > 0.01f)
        {
            _animator.SetBool("Walk", true);
            Vector3 destination = transform.position + transform.right * input.x + transform.forward * input.y;
            _navMeshAgent.destination = destination;
        }
        else
        {
            _navMeshAgent.destination = transform.position;
            _animator.SetBool("Walk", false);
            transform.Rotate(0, input.x * _navMeshAgent.angularSpeed * Time.deltaTime, 0);
        }
    }
}