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

    /// <summary>
    /// Класс позволяет кастомизировать уникальный макет патернов (риссунок из изумрудов) на доске
    /// </summary>
    public class BoardLayout : MonoBehaviour
    {
        #region Variables

        // Позволяет хранить количество рядов для драгоценных камней, является колонкой для хранения рядов или Y высоты макета
        [SerializeField] private RowLayout[] m_allRows;

        #endregion


        #region Properties
        #endregion


        #region Builtin Methods
        #endregion

        #region Custom Methods

        internal Gem[,] GetLayout()
        {
            // Определим ширину и высоту макета доски -  сколько в ряду изумрудов, и максимальную длину всех рядов
            Gem[,] _theLayout = new Gem[m_allRows[0].GemsInRow.Length, m_allRows.Length];

            // заполняющий цикл в обратном порядке
            for(short y = 0; y < m_allRows.Length; y++)
            {
                // m_allRows[y] - позиция на которую мы смотрим 
                for(short x = 0; x < m_allRows[y].GemsInRow.Length; x++)
                {
                    // сначала убедимся что X по какойто причине длинее чем значение ширины макета
                    if (x < _theLayout.GetLength(0))
                    {
                        // Выполним проверку соответствия массива изумрудов во всем ряду
                        if(m_allRows[y].GemsInRow[x] != null)
                        {
                            // чтобы первый драгоценный камень был в нулевой позиции:
                            // Например Y4 в колоне и первый X0 - элемент в ряду изумрудов
                            // 
                            // X - текущий ряд мы знаем но Y - значение должно быть в обратном порядке
                            // В каждой колоне всех первых (нулевых) рядах отнимим одну длину
                            // чтобы начинать счет не с нуля а с еденицы.
                            // После инвертируем в обратном порядке исчисления 
                            // 
                            // Присвоим всем колонкам по Y - текущее значение изумруда в ряду
                            _theLayout[x, m_allRows.Length - 1 -y] = m_allRows[y].GemsInRow[x];
                        }

                    }
                }
            }



            // вернем макет
            return _theLayout;
        }

        #endregion

    }

    /// <summary>
    /// Ксласс позволяет отобразить массив драгоценных камней в ряду 
    /// </summary>
    [System.Serializable]
    internal class RowLayout
    {
        #region Variables

        [SerializeField] private Gem[] m_gemsInRow;

        #endregion


        #region Properties

        /// <summary>
        /// Массив драгоценных камней в ряду. Ряды для хранения изумрудов, являются по X шириной макета
        /// </summary>
        internal Gem[] GemsInRow
        {
            get { return m_gemsInRow; }
        }

        #endregion


    }
}
