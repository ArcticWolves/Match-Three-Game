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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArcticWolves
{
	/// <summary>
	/// Состояния Доски. Позволяет Предотвратить ввод данных игроком, во время движения драгоценных камней
	/// Dynamic - состояние позволяет игроку перемещать изумруды.
	/// Static - состояние ожидания, когда доска занята процессом поиска совпадения и уничтожения изумрудов
	/// </summary>
	internal enum BoardState
    {
		Static,
		Dynamic
    }

	internal class Board : MonoBehaviour
	{
		#region Variables
		[SerializeField] private int m_width = 0;
		[SerializeField] private int m_height = 0;
		
		[SerializeField] private GameObject m_backgroundTilePrefab = null;

		[SerializeField] private float m_gemMovmentSpeed;

		[SerializeField] private Gem[] m_gems = null;
		[SerializeField] private Gem[,] m_allGems = null;

		[SerializeField] private Gem m_bomb = null; // бомба
		[SerializeField] private float m_bombChance = 2f; // шанс выпадения бомбы - по дефолту 2%
		[SerializeField] private float m_bonusMiltiplay; // бонусный множитель очков при последующем совпадении изумрудов
		[SerializeField] private float m_bonusAmount = 0.5f;// количество бонуса, которые получит пользователь, половина от значения

		private bool m_isCheckMatches = true;
		private bool m_isSlideWhenSpawn = true;
		private bool m_isRandomlyWaryaterGem= true;

		private MatchFinder m_matchFinder = null;
		private BoardLayout m_boardLayout = null;
		private Gem[,] m_layoutStore = null;
		private RoundManager m_roundManager = null;


		#endregion


		#region Properties

		internal RoundManager LevelManager
        {
            get{ return m_roundManager; }
        }

		internal Gem Bomb
        {
			get { return m_bomb; }
        }

		/// <summary>
		/// Контролирование текущего состояния доски. Позволяет игроку взаимодействовать с доской когда выбрано - Dynamic
		/// И в ожидании когда - Static
		/// </summary>
		internal BoardState CurrentBoardState = BoardState.Dynamic;
		internal int Height
		{
			get { return m_height; }
		}
		internal int Width
		{
			get { return m_width; }
		}

		/// <summary>
		/// двухмерный масив для изумрудов
		/// </summary>
		internal Gem[,] AllGems
		{
			get { return m_allGems; }
			//set { m_allGems = value; }
		}
		internal MatchFinder FinderOfMatch
        {
			get { return m_matchFinder; }
        }
		internal float GemMovementSpeed
		{
			get { return m_gemMovmentSpeed; }
		}

        #endregion


        #region Builtin Methods

        private void Awake()
        {
			m_matchFinder = FindObjectOfType<MatchFinder>();
			m_roundManager = FindObjectOfType<RoundManager>();
			m_boardLayout = GetComponent<BoardLayout>();
		}

        private void Start()
        {
			m_allGems = new Gem[m_width, m_height];
			m_layoutStore = new Gem[m_width, m_height];

			Setup(m_isRandomlyWaryaterGem, m_isCheckMatches);

			m_matchFinder.FindAllMatches();
		}

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
				SuffleBoard();
            }
        }

        #endregion

        #region Custom Methods


        private void Setup(bool _isRandomVariety, bool _isCheckMatchesOnStart = true)
		{
			// Перед настройкой макета доски выполним проверку, если сылка на макет доски
			if(m_boardLayout != null)
            {
				// Если макет присоединен к доске и существует тогда получи кастомизированый макет и сохрани его 
				m_layoutStore = m_boardLayout.GetLayout();

				// ну, если мы сделаем это c BoardLayout, мы вернем заполненный Gem Array?
				// Верно, это то что мы хотим чтобы произошло.
				// Если это так, так зачем нам хранить m_layoutStore, который является пуст?

				// причина, по которой мы это делаем, заключается в том, что мы собираемся сравнить с нашим последним сохраненным массивом изумрудов
				// и посмотреть, есть ли что - нибудь в нем. И если в нем что - то есть, то мы создадим этот драгоценный камень в том месте, где он есть.
				// Если в нем ничего нет.
				// Тогда мы просто оставим и это как случайный драгоценный камень.
				// Поэтому нам нужно иметь пустое значение по умолчанию, с которым мы можем сравнить на случай, если мы не заполним
				// это внутри, если здесь не осталось доски.

			}

			for (int x = 0; x < m_width; x++)
			{
				for (int y = 0; y < m_height; y++)
				{
					
					Vector2Int _pos = new Vector2Int(x, y);
					int _randomGemIndex = 0;

					// проверим если макет не пустой, чтобы создать собственный дизаин позицый изумрудов на доске
					if(m_layoutStore[x,y] != null)
                    {
						SpawnGem(m_layoutStore[x, y], _pos, m_isSlideWhenSpawn);
                    }
                    else // Если макет пуст, создай изумруды без уникального макета
                    {
						// Если нам нужна вариативность камней
						if (_isRandomVariety)
						{
							_randomGemIndex = Random.Range(0, m_gems.Length);

							// Если мы проверяем совпадения во время старта
							if (_isCheckMatchesOnStart)
							{

								// Имея в списке только 2 типа камня, итерация проверялась бы вечно
								// он всегда будет пытаться выбрать один и тод же камень вечно
								int _whileIteration = 100; // жызненый цикл While, чтобы прекратить бсконечную итерацию совпадения

								// Позиция и драгоценный камень из масива для проверки совпадения. 
								while (MatchesAt(_pos, m_gems[_randomGemIndex]) && _whileIteration < 100)
								{
									// Если мы нашли совпадение камней, перещитаем рандомное число, и таким образом выберим другой камень из массива
									_randomGemIndex = Random.Range(0, m_gems.Length);


									_whileIteration++; // Процесс итерирования цикла
								}
							}
						}

						SpawnGem(m_gems[_randomGemIndex], _pos, m_isSlideWhenSpawn);
					} 

					SpawnBoard(m_backgroundTilePrefab, _pos);

				}
			}

		}


		private void SpawnGem(Gem _gemToSpawn, Vector2Int _pos, bool _isSlide = true)
        {
			// Локальная Конвертация полученого 2D Вектора в 3D вектор
			Vector3 _conVec;
            if (_isSlide)
            {
				_conVec = new Vector3(_pos.x, _pos.y + m_height, 0f);
			}
            else
            {
				_conVec = new Vector3(_pos.x, _pos.y, 0f);
			}

			// Заспавним бомбу случайным образом если она 
			if(Random.Range(0f, 100f) < m_bombChance)
            {
				_gemToSpawn = m_bomb;
            }

			Gem _gem = Instantiate(_gemToSpawn, _conVec, Quaternion.identity);
			_gem.transform.parent = this.transform;
			_gem.name = $"Gem - {_pos.x}, {_pos.y}";

			
			m_allGems[_pos.x, _pos.y] = _gem;

			_gem.SetupGem(_pos, this);
		}

		private void SpawnBoard(GameObject _boardSpawn, Vector2Int _pos)
        {

			Vector3 _conVec = new Vector3(_pos.x, _pos.y, 0f);
			_boardSpawn = Instantiate(_boardSpawn, _conVec, Quaternion.identity);
			_boardSpawn.transform.parent = this.transform;
			_boardSpawn.name = $"Board - {_pos.x}, {_pos.y}";
		}

		/// <summary>
		/// Позволяет отбраковать совпадения в каждой колоне и ряду драгоценных камней в начале уровня игры
		/// </summary>
		/// <param name="_posToCheck"> Позиция для проверки драгоценного камня </param>
		/// <param name="_gemToCheck"> Драгоценый камень который будет случайным образом заменен, в случае если камни совпадают по типу </param>
		/// <returns> Вернет True - Если совпадения обнаружены, вернет False - Если совпадений нет. </returns>
		bool MatchesAt(Vector2Int _posToCheck, Gem _gemToCheck)
        {
			if(_posToCheck.x > 1)
            {
				if(m_allGems[_posToCheck.x -1, _posToCheck.y].TypeOfGem == _gemToCheck.TypeOfGem & m_allGems[_posToCheck.x - 2, _posToCheck.y].TypeOfGem == _gemToCheck.TypeOfGem)
                {
					return true;
                }

            }

			if (_posToCheck.y > 1)
			{
				if (m_allGems[_posToCheck.x, _posToCheck.y -1].TypeOfGem == _gemToCheck.TypeOfGem & m_allGems[_posToCheck.x, _posToCheck.y - 2].TypeOfGem == _gemToCheck.TypeOfGem)
				{
					return true;
				}

			}

			return false;
        }

		/// <summary>
		/// Функция позволяет уничтожить конкретный драгоценный камень который имеет совпадение по типу
		/// </summary>
		/// <param name="_pos"> Текущая позиция драгоценного камня, который должен быть уничтожен</param>
		private void DestroyMatchedGemAt(Vector2Int _pos)
        {
			// проверим существует ли драгоценный камень прежде чем его уничтожать
			if(m_allGems[_pos.x, _pos.y] != null)
            {
				// проверим если драгоценные камни совпадают
				if(m_allGems[_pos.x, _pos.y].IsMatched)
                {
					// Позиция спавна эффекта разрушения.
					Vector2 _spawnEffect = new Vector2(_pos.x, _pos.y);

					// Воспроизведем звук разрушения в зависимости если тип изумруда бомба, камень или обычный изумруд
					if(m_allGems[_pos.x, _pos.y].TypeOfGem == GemType.Bomb)
                    {
						SFXManager.InstanceSFXManager.PlayExplode();
                    }
					else if (m_allGems[_pos.x, _pos.y].TypeOfGem == GemType.Stone)
					{
						SFXManager.InstanceSFXManager.PlayStoneBreake();
					}
                    else
                    {
						SFXManager.InstanceSFXManager.PlayGemBrake();
					}

					// Создадим эфект частиц разрушения, который в следствии сам себя уничтожет при павершении анимации
					Instantiate(m_allGems[_pos.x, _pos.y].DestroyEffect, _spawnEffect, Quaternion.identity);
					// Уничтожем игровой обьект в конкретной позиции
					Destroy(m_allGems[_pos.x, _pos.y].gameObject);

					// Обновим массив совпадений, и скажем что внем ничего нет. Так как обьекты были уничтожены
					m_allGems[_pos.x, _pos.y] = null;
                }
            }
        }

		/// <summary>
		/// Функция позволяет уничтожить любые совпадения драгоценных камней
		/// </summary>
		internal void DestroyMatches()
        {
			for(int i = 0; i < m_matchFinder.CurrentFindedMatches.Count; i++)
            {
				// Убедимся и проверим если список совпадений не пуст
				if (m_matchFinder.CurrentFindedMatches[i] != null)
                {
					// Прибавим очков от разрушения текущего совпадения 
					ScoreCheck(m_matchFinder.CurrentFindedMatches[i]);


					// Уничтожем текущие найденые совпадения камней
					DestroyMatchedGemAt(m_matchFinder.CurrentFindedMatches[i].CurrentPosition);
				}
            }

			// После унитожения изумрудов, определите пустые позиции и переместите изумружы в каждой колоне
			StartCoroutine(DecreaseRowCoroutine());
        }

		private IEnumerator DecreaseRowCoroutine()
		{
			// Создадим задержку после взрыва изумрудов
			yield return new WaitForSeconds(0.2f);

			// Одследим количество пустых мест 
			int _nullCounter = 0;

			// Создадим цикл, который будет находить пустые ячейки изумрудов в Ряду
			for (int x = 0; x < Width; x++)
			{
				// Создадим цикл, который будет находить пустые ячейки изумрудов в колоне
				for (int y = 0; y < Height; y++)
				{
					// Текущий проверяемый изумруд в 2D массиве 
					Gem _currGem = m_allGems[x, y];
					
					// Убедимся что изумруд на который мы проверяем равен Null и являеться пустым местом
					if (_currGem == null)
                    {
						// Если есть пустое место, увеличим Счетчик индекса 
						_nullCounter++;
                    }
					else if (_nullCounter > 0)
                    {
						// Таким образом мы убераем у текушей позиции  изумруда спомощу счетчика
						int _currPosY = _currGem.CurrentPosition.y - _nullCounter;

						// Создадим целевую позицию для перемещения.
						Vector2Int _targetPosition = new Vector2Int(_currGem.CurrentPosition.x, _currPosY);

						// нам нужно сказать нашему изумруду переместиться в эту пустую позицию В КОЛОНЕ ИЕРТИКАЛЬНО
						_currGem.CurrentPosition = _targetPosition;

						// Присвоем текущую позицию всем изумрудам в массиве
						m_allGems[x, y - _nullCounter] = _currGem;

						m_allGems[x, y] = null;

					}

				}

				_nullCounter = 0;

			}

			// Заполняет доску изумрудами
			StartCoroutine(FillBoardCoroutine());

		}


		private IEnumerator FillBoardCoroutine()
        {
			yield return new WaitForSeconds(0.5f);
			RefillBoard();

			// Для того чтобы происсходил каскадный эффект
			// Проверка на совпадения после заполнения доски
			yield return new WaitForSeconds(0.5f);
			m_matchFinder.FindAllMatches();

			if(m_matchFinder.CurrentFindedMatches.Count > 0)
            {
				yield return new WaitForSeconds(0.5f);
				// перед уничтожения совпадений, прибавим бонусный множитель на 1
				m_bonusMiltiplay++;

				DestroyMatches();

			}
            else // иначе если совпадений не осталось
            {
				yield return new WaitForSeconds(0.5f);
				CurrentBoardState = BoardState.Dynamic;

				//если мы не находим больше совпадений, сбросим множитель в ноль
				m_bonusMiltiplay = 0f;
			}

		}


		/// <summary>
		/// Позволяет пере заполнить доску изумрудами, если есть свободные ячейки
		/// </summary>
		private void RefillBoard()
        {
			for(int x = 0; x < m_width; x++)
            {
				for (int y = 0; y < m_height; y++)
                {
					// Проверим нет ли пустот на доске
					// Если В массиве изумрудов имеется null
					if(AllGems[x,y] == null)
                    {
						Vector2Int _pos = new Vector2Int(x, y);
						int _randomGemIndex = Random.Range(0, m_gems.Length);
						SpawnGem(m_gems[_randomGemIndex], _pos);
					}
					

					
				}

			}

			CheckMisPlacedGems();
        }

		/// <summary>
		/// Позволяет уничтожить любые обнаруженые дубликаты джемов вне доски 
		/// </summary>
		private void CheckMisPlacedGems()
        {
			// список найденых Дубликатов
			List<Gem> _foundDublicateGem = new List<Gem>();

			_foundDublicateGem.AddRange(FindObjectsOfType<Gem>());

			// Циклическим поиском найдем Дубликаты на доске 
			for(int x = 0; x < m_width; x++)
			{
				for (int y = 0; y < m_height; y++)
				{
					if (_foundDublicateGem.Contains(m_allGems[x, y]))
                    {
						_foundDublicateGem.Remove(m_allGems[x, y]);
                    }
                }

			}

			foreach(Gem _gem in _foundDublicateGem)
            {
				Destroy(_gem.gameObject);
            }

        }

		/// <summary>
		/// Функция позволяет перетасовать доску с изумрудами, в случае если совпадений не обнаружено, 
		/// и игрок не может найти три совпадения 
		/// </summary>
		internal void SuffleBoard()
        {
			if(CurrentBoardState != BoardState.Static)
            {
				CurrentBoardState = BoardState.Static;

				// получить все драгоценные камни, которые в настоящее время существуют на доске.
				List<Gem> _gemsFromBoard = new List<Gem>();

				// Затем мы просто пройдемся по всей нашей доске.
				// Этот цикл будет удалять ссылку на каждый изумруд на доске
				for(int x = 0; x < m_width; x++)
                {
					for(int y = 0; y < m_height; y++)
                    {
						// Добавим в список все изумруды которые на доске
						_gemsFromBoard.Add(m_allGems[x, y]);

						// скажем что на доске нет изумрудов, и доска пустая
						// таким образом мы не удаляем все изумруды из доски,
						// а просто отсоединяем сылку на изумруд
						m_allGems[x, y] = null;
                    }
                }

				// нам нужно чтобы доска была полностью пустой.
				// прежде чем мы сможем заполнить его снова.
				// на нашей доске теперь нет не одного назначеного изумруда
				// теперь нам нужно добавить изумруды обратно в массив один за другим
				// этот цикл будет возвращать изумруды на доску
				for (int x = 0; x < m_width; x++)
				{
					for (int y = 0; y < m_height; y++)
					{
						// выбери случайный изумруд для помещения его на доску
						int _currRandGemIndex = Random.Range(0, _gemsFromBoard.Count);

						// выберем позицию для изумруда текущего цикла
						Vector2Int _currGemPos = new Vector2Int(x, y) ;

						// создадим итерацию, для ограничения чтобы цикл While не работал вечно
						// и не пытался находить совпадения на последнем изумруде
						int _iterationLoop = 0;

						// проверим совпадения текущей позиции, и случайного изумруда из списка на совпадения
						// проверим если итерация цикла не достигла 100 
						// Проверим также если у нас больше чем один изумруд, мы можем выбрать случайный  
						while (MatchesAt(_currGemPos, _gemsFromBoard[_currRandGemIndex]) && _iterationLoop < 100 && _gemsFromBoard.Count > 1)
                        {
							// если совпадение изумрудов было, выберем другой случайный изумруд
							_currRandGemIndex = Random.Range(0, _gemsFromBoard.Count);

							// Прибавим итерацию 
							_iterationLoop++;
						}

						// установим новую позициию в масив, от случайно сгенерированого изумруда
						_gemsFromBoard[_currRandGemIndex].SetupGem(_currGemPos, this);

						// помести этот камень в этот слот на доске
						m_allGems[x, y] = _gemsFromBoard[_currRandGemIndex];

						// затем удалим сгенерированый изумруд из списка
						_gemsFromBoard.RemoveAt(_currRandGemIndex);
					}
				}


				StartCoroutine(FillBoardCoroutine());
			}
		}

		private void ScoreCheck(Gem _gemToCheck)
        {
			m_roundManager.CurrentScore += _gemToCheck.ScoreValue;

			// Проверим есть ли у нас бонусный множитель
			if(m_bonusMiltiplay > 0f)
            {
				float _bonusToAdd = _gemToCheck.ScoreValue * m_bonusMiltiplay * m_bonusAmount;

				// добавим екстра бонус к текущим очкам
				m_roundManager.CurrentScore += Mathf.RoundToInt(_bonusToAdd);
            }

        }

		#endregion

	}
}
