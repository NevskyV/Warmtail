using Data.Player;
using Entities.UI;
using Interfaces;
using Zenject;

namespace Entities.Props
{
    public class StarTutorial : SavableStateObject, IInteractable
    {
        
        [Inject]
        public void Construct(MonologueVisuals monologueVisuals)
        {
        }
    
        public void Interact()
        {
            if (_globalData == null) return;
            _globalData.Edit<SavablePlayerData>((playerData) =>
            {
                playerData.Stars += 1;
            });
            ChangeState(false);
        }
    }
}
