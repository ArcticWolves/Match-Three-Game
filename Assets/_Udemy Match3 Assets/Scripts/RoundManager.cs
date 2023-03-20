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

namespace ArcticWolves
{
	public class RoundManager : MonoBehaviour
	{
		#region Variables
		[SerializeField] private float m_roundTime = 60f;
		private UIManager m_uiManager;
        private Board m_boardOfGems;
        private bool m_isRoundEnd = false;
        private int m_currentScore = 0;

        private float m_displayScore;

        [SerializeField] private int m_scoreTarget1 = 100;
        [SerializeField] private int m_scoreTarget2 = 500;
        [SerializeField] private int m_scoreTarget3 = 1000;

        [SerializeField] private float m_smothSpeedDisplay;
        #endregion


        #region Properties

        /// <summary>
        /// Текущее количество очков на уровне
        /// </summary>
        internal int CurrentScore
        {
            get { return m_currentScore; }
            set { m_currentScore = value; }
        }

        /// <summary>
        /// Время на текущем раунде
        /// </summary>
        internal float TimeOnTheRound
        {
            get { return m_roundTime; }
        }

        /// <summary>
        /// Значение позволяет указать не завершен ли уровень
        /// </summary>
        internal bool IsRoundEnd
        {
            get { return m_isRoundEnd; }
        }
        #endregion


        #region Builtin Methods
        private void Awake()
        {
			m_uiManager = FindObjectOfType<UIManager>();
            m_boardOfGems = FindObjectOfType<Board>();
        }

        private void Start()
        {
            
        }

        private void Update()
        {
            // Если наше оставшееся время на уровне больше нуля
            // отнимим время 
            if(m_roundTime > 0)
            {
                m_roundTime = m_roundTime - Time.deltaTime;

                // Если по какойто причине время меньше нуля или равно нулю
                // время раунда всегда будет ноль
                if(m_roundTime <= 0f)
                {
                    m_roundTime = 0f;

                    m_isRoundEnd = true;
                }
            }

            if(m_isRoundEnd && m_boardOfGems.CurrentBoardState == BoardState.Dynamic)
            {
                WindowPanelsChacker();
                m_isRoundEnd = false;
            }




            m_uiManager.TimeRemainingText.text = m_roundTime.ToString("0.0") + "s";

            m_displayScore = Mathf.Lerp(m_displayScore, m_currentScore, m_smothSpeedDisplay * Time.deltaTime);
            m_uiManager.ScoreValueText.text = m_displayScore.ToString("0");
        }

        #endregion

        #region Custom Methods

        private void WindowPanelsChacker()
        {
            m_uiManager.RoundIsOverScreen.SetActive(true);

            m_uiManager.FInalScoreText.text = m_currentScore.ToString();

            if(m_currentScore >= m_scoreTarget3)
            {
                m_uiManager.RoundIsOverTitleText.text = "Congratulations! You earned 3 stars";
                m_uiManager.PanelStars3.SetActive(true);
            }
            else if(m_currentScore >= m_scoreTarget2)
            {
                m_uiManager.RoundIsOverTitleText.text = "Congratulations! You earned 2 stars";
                m_uiManager.PanelStars2.SetActive(true);
            }
            else if(m_currentScore >= m_scoreTarget1)
            {
                m_uiManager.RoundIsOverTitleText.text = "Congratulations! You earned 1 star";
                m_uiManager.PanelStars1.SetActive(true);
            }
            else
            {
                m_uiManager.RoundIsOverTitleText.text = "Oh no, No stars for you! Try again?";
            }

        }

        #endregion

    }
}
