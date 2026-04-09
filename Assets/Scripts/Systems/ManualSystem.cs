using System.Collections.Generic;
using Data;
using Entities.UI;
using Zenject;

namespace Systems
{
    public class ManualSystem
    {
        [Inject] private GlobalData _globalData;

        public List<int> GetUnlockedPages(Chapter chapter)
        {
            switch (chapter)
            {
                case Chapter.World:
                    return _globalData.Get<WorldData>().ManualWorld;
                case Chapter.Characters:
                    return _globalData.Get<WorldData>().ManualCharacter;
                case Chapter.Puzzles:
                    return _globalData.Get<WorldData>().ManualPuzzle;
            }

            return new();
        }

        public void AddPage(Chapter chapter, int page)
        {
            switch (chapter)
            {
                case Chapter.World:
                    _globalData.Edit<WorldData>(x => x.ManualWorld.Add(page));
                    break;
                case Chapter.Characters:
                    _globalData.Edit<WorldData>(x => x.ManualCharacter.Add(page));
                    break;
                case Chapter.Puzzles:
                    _globalData.Edit<WorldData>(x => x.ManualPuzzle.Add(page));
                    break;
            }
        }
    }
}