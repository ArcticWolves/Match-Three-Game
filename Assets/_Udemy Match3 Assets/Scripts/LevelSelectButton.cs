#region Copyright
/* Этот код защищен авторским правом и управляеться лицензией GPL3.0
 * https://www.gnu.org/licenses/gpl-3.0.html
 * 
 *      _    ____   ____ _____ ___ ____  __        _____  _ __     _______ ____  
 *     / \  |  _ \ / ___|_   _|_ _/ ___| \ \      / / _ \| |\ \   / / ____/ ___| 
 *    / _ \ | |_) | |     | |  | | |      \ \ /\ / / | | | | \ \ / /|  _| \___ \ 
 *   / ___ \|  _ <| |___  | |  | | |___    \ V  V /| |_| | |__\ V / | |___ ___) |
 *  /_/   \_\_| \_\\____| |_| |___\____|    \_/\_/  \___/|_____\_/  |_____|____/ 
 * 
 *  Copyright (c) Arctic Wolves LLC - Roman K.
 */
#endregion
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ArcticWolves
{
    public class LevelSelectButton : MonoBehaviour
    {
        #region Variables

        [SerializeField] private string m_levelToLoad;

        [SerializeField] private GameObject m_star1;
        [SerializeField] private GameObject m_star2;
        [SerializeField] private GameObject m_star3;
        #endregion


        #region Properties
        #endregion


        #region Builtin Methods
        private void Start()
        {
        #endregion

        #region Custom Methods
        public void LoadLevel()
        {
            SceneManager.LoadScene(m_levelToLoad);
            #endregion
        }
    }
}
