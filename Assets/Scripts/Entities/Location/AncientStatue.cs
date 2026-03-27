using Cysharp.Threading.Tasks;
using Entities.Props;
using Interfaces;

namespace Entities.Location
{
    public class AncientStatue : SavableStateObject, IInteractable
    {
        private const int _effectTime = 180;
        public async void Interact()
        {
            //TODO: in Warmth system disable drain of cells
            await UniTask.WaitForSeconds(_effectTime);
            //TODO: in Warmth system enable drain of cells
            ChangeState(false);
        }
    }
}