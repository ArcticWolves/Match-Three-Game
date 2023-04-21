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
using TMPro;
using UnityEngine.SceneManagement;

namespace ArcticWolves
{
    public class UIManager : MonoBehaviour
    {
        #region Variables

        [SerializeField] private TMP_Text m_timeRemainingText;
        [SerializeField] private TMP_Text m_scoreValueText;
        [SerializeField] private GameObject m_roundIsOverScreen;

        [SerializeField] private TMP_Text m_finalScoreText;
        [SerializeField] private TMP_Text m_roundIsOverText;

        [SerializeField] private GameObject m_panelStars1;
        [SerializeField] private GameObject m_panelStars2;
        [SerializeField] private GameObject m_panelStars3;
        [SerializeField] private GameObject m_panelPauseScreen;

        private Board m_board;
        [SerializeField] private string m_levelSelect = "LevelSelect";
        internal static string m_webplayerQuitURL = "https://arcticwolves.games";
        #endregion


        #region Properties

        /// <summary>
        /// Панель с тремя звёздами
        /// </summary>
        internal GameObject PanelStars3
        {
            get { return m_panelStars3; }
        }

        /// <summary>
        /// Панель с двумя звездами
        /// </summary>
        internal GameObject PanelStars2
        {
            get { return m_panelStars2; }
        }

        /// <summary>
        /// Панель с одной звездой
        /// </summary>
        internal GameObject PanelStars1
        {
            get { return m_panelStars1; }
        }


        /// <summary>
        /// Текст позволяет сообщить игроку о получении звёзд или конца уровня
        /// </summary>
        internal TMP_Text RoundIsOverTitleText
        {
            get { return m_roundIsOverText; }
        }

        /// <summary>
        /// Текст финальных очков получаемый в конце раунда
        /// </summary>
        internal TMP_Text FInalScoreText
        {
            get { return m_finalScoreText; }
        }

        /// <summary>
        /// UI Текстовое поле остатка времени на уровне
        /// </summary>
        internal TMP_Text TimeRemainingText
        {
            get { return m_timeRemainingText; }
        }

        /// <summary>
        /// Текстовое поле количества очков пользователя на UI
        /// </summary>
        internal TMP_Text ScoreValueText
        {
            get { return m_scoreValueText; }
        }

        /// <summary>
        /// Панель конца уровня
        /// </summary>
        internal GameObject RoundIsOverScreen
        {
            get { return m_roundIsOverScreen; }
        }


        #endregion


        #region Builtin Methods

        private void Awake()
        {
            m_board = FindObjectOfType<Board>();
        }


        private void Start()
        {
            m_panelStars1.SetActive(false);
            m_panelStars2.SetActive(false);
            m_panelStars3.SetActive(false);


        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                QuitFromGame();
            }
        }
        #endregion

        #region Custom Methods

        public void TryAgain()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void PauseAndUnPause()
        {
            if (!m_panelPauseScreen.activeInHierarchy)
            {
                m_panelPauseScreen.SetActive(true);
                Time.timeScale = 0f;
            }
            else
            {
                m_panelPauseScreen.SetActive(false);
                Time.timeScale = 1f;
            }
        }

        public void ShuffleBoard()
        {
            m_board.SuffleBoard();

        }

        public void GoToLevelSelectMenu()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(m_levelSelect);
        }

        public void QuitFromGame()
        {
#if (UNITY_EDITOR || DEVELOPMENT_BUILD)
            Debug.Log(this.name + " : " + this.GetType() + " : " + System.Reflection.MethodBase.GetCurrentMethod().Name);
#endif

#if (UNITY_EDITOR)
            UnityEditor.EditorApplication.isPlaying = false;
#elif (UNITY_STANDALONE) 
            Application.Quit();
#elif (UNITY_WEBGL)
            Application.OpenURL("m_webplayerQuitURL");
#endif
        }

        #endregion

    }
}
