using System.Collections.Generic;
using UnityEngine;

namespace FrogLibrary
{
    public abstract class ConstantTable : ScriptableObject
    {
        public abstract void Load(IReadOnlyDictionary<int, IReadOnlyDictionary<int, IReadOnlyList<object>>> excel);

        /*

         foreach ((int sheetIndex, IReadOnlyDictionary<int, IReadOnlyList<object>> sheetData) in excel)
            {
                foreach ((int row, IReadOnlyList<object> cols) in sheetData)
                {

                }
            }

         */
    }
}