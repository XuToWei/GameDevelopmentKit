using QFSW.QC.Actions;
using System.Collections.Generic;
using UnityEngine;

namespace QFSW.QC.Demo
{
    [CommandPrefix("demo.robot.")]
    public class Robot : MonoBehaviour
    {
        [SerializeField] private GameObject deathFX = null;

        [Command("speed")]
        private static float robotSpeed = 25f;

        [Command("rotation-speed")]
        private static float robotRotationSpeed = 40f;

        private Vector2 direction;

        private void Start()
        {
            SpriteRenderer rend = GetComponent<SpriteRenderer>();
            rend.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1);

            float size = Random.Range(0.8f, 1.2f);
            transform.localScale = new Vector3(size, size, size);

            direction = Quaternion.Euler(0, 0, Random.Range(0, 306f)) * Vector3.up;
        }

        private void FixedUpdate()
        {
            GetComponent<Rigidbody2D>().AddForce(direction * robotSpeed * Time.fixedDeltaTime, ForceMode2D.Force);
            direction = Quaternion.Euler(0, 0, robotRotationSpeed * Time.fixedDeltaTime) * direction;
        }

        [Command("kill")]
        private static IEnumerator<ICommandAction> KillAction()
        {
            Robot robot = default;
            IEnumerable<Robot> robots = InvocationTargetFactory.FindTargets<Robot>(MonoTargetType.All);

            yield return new Value("Please select a robot");
            yield return new Choice<Robot>(robots, r => robot = r);
            
            robot.Die();
            yield return new Typewriter($"{robot.name} has been killed");
        }

        [Command("kill-all", MonoTargetType.All)]
        public void Die()
        {
            Destroy(gameObject);
            Destroy(Instantiate(deathFX, transform.position, Quaternion.identity), 3);
        }

        [Command("position", MonoTargetType.All)]
        private Vector3 GetPosition()
        {
            return transform.position;
        }
    }
}
