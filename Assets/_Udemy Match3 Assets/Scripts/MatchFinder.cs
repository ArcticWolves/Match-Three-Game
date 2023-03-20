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
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ArcticWolves
{
	internal class MatchFinder : MonoBehaviour
	{
		#region Variables

		private Board m_Board; // Доска для поиска совпадений изумрудов
        [SerializeField] private List<Gem> m_currentFindedMatches = new List<Gem>(); // should use HashSet<Gem> instead of List?
        #endregion

        #region Properties
        internal List<Gem> CurrentFindedMatches
        {
            get { return m_currentFindedMatches; }
        }
        #endregion

        #region Builtin Methods
        // Используем функцию пробуждения вместо старта, позволит нам найти доску первым делом
        private void Awake()
        {
            m_Board = FindObjectOfType<Board>();
        }

        #endregion

        #region Custom Methods

        /// <summary>
        /// Циклический Поиск всех совпадений с драгоценными камнями на доске,
        /// проверяет каждый драгоценный камень по вертикали и горизонтали
        /// </summary>
        internal void FindAllMatches()
        {
            m_currentFindedMatches.Clear();

            for(int x = 0; x < m_Board.Width; x++)
            {
                for(int y = 0; y < m_Board.Height; y++)
                {
                    Gem _currentGem = m_Board.AllGems[x, y];
                    // Убедимся в том что камень который мы ищем не отсутствует (пустая ячейка)
                    // ничего не произойдет если в слоте нет камня
                    if(_currentGem != null)
                    {
                        // Если в нашем слоте есть камень найди совпадения ПО ГОРИЗОНТАЛИ 
                        // Проверим не на краю доски
                        if (x > 0 && x < m_Board.Width - 1)
                        {
                            // пока мы не достигли етих краёв определим камни по горизонтали
                            Gem _leftGem = m_Board.AllGems[x - 1, y];
                            Gem _rightGem = m_Board.AllGems[x + 1, y];

                            // Убедимся что оба этих драгоценных камня существуют и не пусты
                            if(_leftGem & _rightGem != null)
                            {
                                // сравним и определим совпадают ли Тип камня слева и справа
                                if(_leftGem.TypeOfGem == _currentGem.TypeOfGem & _rightGem.TypeOfGem == _currentGem.TypeOfGem)
                                {
                                    // если совпадения есть 
                                    _currentGem.IsMatched = true;
                                    _leftGem.IsMatched = true;
                                    _rightGem.IsMatched = true;

                                    // Отследим совпадения ПО ВЕРТЕКАЛИ и добавим драгоценные камни в список совпадений
                                    m_currentFindedMatches.Add(_currentGem);
                                    m_currentFindedMatches.Add(_leftGem);
                                    m_currentFindedMatches.Add(_rightGem);

                                }
                            }
                        }

                        // ПО ВЕРТИКАЛИ 
                        // Проверим не на краю доски
                        if (y > 0 && y < m_Board.Height - 1)
                        {
                            // пока мы не достигли етих краёв определим камни по горизонтали
                            Gem _aboveGem = m_Board.AllGems[x , y + 1];
                            Gem _belowGem = m_Board.AllGems[x , y - 1];

                            // Убедимся что оба этих драгоценных камня существуют и не пусты
                            if (_aboveGem & _belowGem != null)
                            {
                                // сравним и определим совпадают ли Тип камня СВЕРХУ и СНИЗУ
                                if (_aboveGem.TypeOfGem == _currentGem.TypeOfGem & _belowGem.TypeOfGem == _currentGem.TypeOfGem)
                                {
                                    // если совпадения есть 
                                    _currentGem.IsMatched = true;
                                    _aboveGem.IsMatched = true;
                                    _belowGem.IsMatched = true;

                                    // Отследим совпадения ПО ВЕРТЕКАЛИ и добавим драгоценные камни в список совпадений
                                    m_currentFindedMatches.Add(_currentGem);
                                    m_currentFindedMatches.Add(_aboveGem);
                                    m_currentFindedMatches.Add(_belowGem);
                                }
                            }
                        }

                    }
                }
            }

            

            // если в список добавляються камни совпадения и их больше чем 0(1)
            if(m_currentFindedMatches.Count > 0)
            {
                // из за постоянного потока добавления в список, обьекты могут дублироваться
                // извлеки из списка совпадений уникальные обьекты(не дубликаты)
                // Distinct - позволяет создать дублируемый список в который будут помещены уникальные обьекты
                m_currentFindedMatches = m_currentFindedMatches.Distinct().ToList();
            }

            CheckForBombs();
        }

        /// <summary>
        /// Функция позволяет Проверить найденые совпадения, на наличие бомбы 
        /// </summary>
        internal void CheckForBombs()
        {
            for(int i = 0; i < m_currentFindedMatches.Count; i++)
            {
                Gem _checkedGem = m_currentFindedMatches[i];

                // Позииции которые будут проверены на наличии рядом бомбы
                int _xPos = _checkedGem.CurrentPosition.x;
                int _yPos = _checkedGem.CurrentPosition.y;


                // убедимся в том чтомы не на краю доски СПРАВА от проверяемого изумрудного камня
                if (_checkedGem.CurrentPosition.x > 0)
                {
                    // Убедимя если СЛЕВА отпроверяемого камня существует изумруд 
                    if (m_Board.AllGems[_xPos - 1, _yPos] != null)
                    {
                        // Если СЛЕВА от нашего изумруда нахродится бомба
                        if(m_Board.AllGems[_xPos - 1, _yPos].TypeOfGem == GemType.Bomb)
                        {
                            MarkBombArea(new Vector2Int(_xPos - 1, _yPos), m_Board.AllGems[_xPos - 1,_yPos]); // пометь эту зону бомбой
                        }
                    }
                }



                // убедимся в том что мы не на краю доски СЛЕВА от проверяемого изумрудного камня  
                if (_checkedGem.CurrentPosition.x < m_Board.Width - 1)
                {
                    // Проверить ПРАВУЮ сторону от проверяемого камня
                    if (m_Board.AllGems[_xPos + 1, _yPos] != null)
                    {
                        if (m_Board.AllGems[_xPos + 1, _yPos].TypeOfGem == GemType.Bomb)
                        {
                            MarkBombArea(new Vector2Int(_xPos + 1, _yPos), m_Board.AllGems[_xPos + 1, _yPos]); // пометь эту зону бомбой
                        }
                    }
                }



                // убедимся в что мы не на краю доски СВЕРХУ от проверяемого изумрудного камня
                if (_checkedGem.CurrentPosition.y > 0)
                {
                    // Убедимя если СНИЗУ отпроверяемого камня существует изумруд 
                    if (m_Board.AllGems[_xPos, _yPos - 1] != null)
                    {
                        // Если СЛЕВА от нашего изумруда нахродится бомба
                        if (m_Board.AllGems[_xPos, _yPos - 1].TypeOfGem == GemType.Bomb)
                        {
                            MarkBombArea(new Vector2Int(_xPos, _yPos - 1), m_Board.AllGems[_xPos, _yPos - 1]); // пометь эту зону бомбой
                        }
                    }
                }




                // убедимся в том чтомы не на краю доски СНИЗУ от проверяемого изумрудного камня
                if (_checkedGem.CurrentPosition.y < m_Board.Height - 1)
                {
                    // Проверим если СВЕРХУ НАД проверяемым камнем существует изумруд 
                    if (m_Board.AllGems[_xPos, _yPos + 1] != null)
                    {
                        // Если СВЕРХУ от нашего изумруда нахродится бомба
                        if (m_Board.AllGems[_xPos, _yPos + 1].TypeOfGem == GemType.Bomb)
                        {
                            MarkBombArea(new Vector2Int(_xPos, _yPos + 1), m_Board.AllGems[_xPos, _yPos + 1]); // пометь эту зону бомбой
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Функция позволяет определить бомбу и его позицию
        /// </summary>
        /// <param name="_bombPos"> Позиция от которой будет осуществлен взрыв </param>
        /// <param name="_theBomb"> Текущая бомба </param>
        internal void MarkBombArea(Vector2Int _bombPos, Gem _theBomb)
        {
            // Мы должны перебрать все драгоценные камни которые находятся вокруг нас по X и Y позиций
            for(int x = _bombPos.x - _theBomb.BlastSize; x <= _bombPos.x + _theBomb.BlastSize; x++)
            {
                // Тоже самое и по оси Y
                for(int y = _bombPos.y - _theBomb.BlastSize; y <= _bombPos.y + _theBomb.BlastSize; y++)
                {

                    // Итак, здесь нам нужно сделать проверку и посмотреть,
                    // находится ли точка, на которую мы смотрим, в пределах потенциального радиуса доски.
                    if((x >= 0 && x < m_Board.Width) && (y >= 0 && y < m_Board.Height))
                    {
                        // Хорошо, теперь мы знаем, что на данный момент смотрим на конкретный драгоценный камень.
                        // создадим проверку и проверим если камень не являеться null 
                        if(m_Board.AllGems[x,y] != null)
                        {
                            // все обнаруженые камни будут совпадать
                            m_Board.AllGems[x, y].IsMatched = true;


                            m_currentFindedMatches.Add(m_Board.AllGems[x, y]);
                        }
                    }

                }
            }
            // Поместим текущие найденые совпадения в в уникальный список без дубликатов джемов
            m_currentFindedMatches = m_currentFindedMatches.Distinct().ToList();
        }

        #endregion

    }
}
