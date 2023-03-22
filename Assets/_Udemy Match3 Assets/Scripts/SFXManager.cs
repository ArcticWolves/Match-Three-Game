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
    public class SFXManager : MonoBehaviour
    {
        #region Variables

        private static SFXManager m_instance;

        [SerializeField] private AudioSource m_gemSound;
        [SerializeField] private AudioSource m_explodeSound;
        [SerializeField] private AudioSource m_stoneSound;
        [SerializeField] private AudioSource m_roundOverSound;
        #endregion


        #region Properties
        internal static SFXManager InstanceSFXManager
        {
            get { return m_instance; }
        }
        #endregion


        #region Builtin Methods

        private void Awake()
        {
            m_instance = this;
        }

        #endregion

        #region Custom Methods

        internal void PlayGemBrake()
        {
            m_gemSound.Stop();

            m_gemSound.pitch = Random.Range(0.8f, 1.2f);

            m_gemSound.Play();
        }

        internal void PlayExplode()
        {
            m_explodeSound.Stop();

            m_explodeSound.pitch = Random.Range(0.8f, 1.2f);

            m_explodeSound.Play();
        }

        internal void PlayStoneBreake()
        {
            m_stoneSound.Stop();

            m_stoneSound.pitch = Random.Range(0.8f, 1.2f);

            m_stoneSound.Play();
        }

        internal void PlayRoundOver()
        {
            m_roundOverSound.Play();
        }

        #endregion

    }
}
