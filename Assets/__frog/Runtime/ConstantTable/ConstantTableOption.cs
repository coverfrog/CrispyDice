using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FrogLibrary
{
    public class ConstantTableOption 
    {
        [Header("# Constant Table")] 
        public string m_excelFolderPath = "Assets/";
        public string m_tableFolderPath = "Assets/";
        public string m_namespace = "";
        public List<ConstantNameMatch> m_matches = new List<ConstantNameMatch>();

        public string ExcelFolderPath => m_excelFolderPath;
        
        public string TableFolderPath => m_tableFolderPath;

        public string Namespace => m_namespace;
        
        public IReadOnlyDictionary<string, ConstantNameMatch> Matches => m_matches.ToDictionary(m => m.excelName);
    }
}