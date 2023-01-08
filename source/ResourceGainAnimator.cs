using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Steel;
using SteelCustom.UIElements;

namespace SteelCustom
{
    public class ResourceGainAnimator : ScriptComponent
    {
        private Map _map;
        private Queue<(Hex Hex, ResourceType ResourceType)> _queue = new Queue<(Hex Hex, ResourceType ResourceType)>();
        private float _timer = 0.0f;
        private const float DELAY = 0.25f;

        public override void OnUpdate()
        {
            _timer += Time.DeltaTime;
            if (_timer > DELAY && _queue.Any())
            {
                _timer = 0.0f;
                (Hex Hex, ResourceType ResourceType) pair = _queue.Dequeue();
                AnimateInner(pair.Hex, pair.ResourceType);
            }
        }

        public void Init(Map map)
        {
            _map = map;
        }

        public void Animate(Hex hex, ResourceType resourceType)
        {
            _queue.Enqueue((hex, resourceType));
        }

        private void AnimateInner(Hex hex, ResourceType resourceType)
        {
            Entity entity = new Entity("Resource", Entity);
            entity.AddComponent<SpriteRenderer>().Sprite = ResourcesManager.GetImage(UIManager.ResourceIndexToSpritePath((int)resourceType));
            entity.Transformation.Position = hex.Transformation.Position + new Vector3(0, 0, 2);
            
            entity.AddComponent<AudioSource>().Play(ResourcesManager.GetAudioTrack("gain_resource.wav"));

            StartCoroutine(AnimateCoroutine(entity));
        }

        private IEnumerator AnimateCoroutine(Entity entity)
        {
            float timer = 0.0f;
            while (timer < 3.0f)
            {
                entity.Transformation.Position += new Vector3(0, Time.DeltaTime * 3.0f, 0);
                
                timer += Time.DeltaTime;
                yield return null;
            }
            
            entity.Destroy(3.0f);
        }
    }
}