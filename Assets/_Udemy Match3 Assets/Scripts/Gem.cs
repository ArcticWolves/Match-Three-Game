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
using UnityEngine;

namespace ArcticWolves
{
	internal enum GemType
    {
		Blue,
		Green,
		Red,
		Yellow,
		Purple,
		Bomb,
		Stone
    }

	internal class Gem : MonoBehaviour
	{
        #region Variables

        private Vector2Int m_currentPos = Vector2Int.zero;  
		private Vector2Int m_previousePos = Vector2Int.zero; // Предидущая Позиция драгоценного камня (Если нет совпадения)

		private Board m_boardOfGems = null; 

		private Vector2 m_firstTouchPosition = Vector2.zero; // Первая позиция свапа
		private Vector2 m_lastTouchPosition = Vector2.zero; // заключающая позиция свапа
		

		private bool m_isMouseButtonPressed = false; // удерживает ли пользователь кнопку мыши 
		private bool m_isMatched = false;  
		
		private float m_swipeAngle = 0f; // Угол направления взмаха 

		[SerializeField] private GemType m_gemType; 

		[SerializeField] private GameObject m_destroyParticleEffect; 

		[SerializeField] private int m_blastSize = 2;
		[SerializeField] private int m_scoreValue = 10;

		private Gem m_targetGem = null; // целевой кристал для замены с текущим
		private Gem m_currentGem = null; // Текущий кристал 

		#endregion

		#region Properties

		/// <summary>
		/// Поинты - значения одного изумруда, которые получит игрок, в случае разрушения изумрудов
		/// </summary>
		internal int ScoreValue
        {
            get { return m_scoreValue; }
        }

		/// <summary>
		/// Радиус поражения бомбой рядом камней 
		/// </summary>
		internal int BlastSize
        {
            get { return m_blastSize; }
        }

		/// <summary>
		/// Текущая позиция драгоценного камня на доске
		/// </summary>
		internal Vector2Int CurrentPosition
        {
			get { return m_currentPos; }
			set { m_currentPos = value; }
        }

		/// <summary>
		/// Партиклы - эффект частиц разрушения
		/// </summary>
		internal GameObject DestroyEffect
        {
            get { return m_destroyParticleEffect; }
        }

		/// <summary>
		/// True - Если Совпал этот драгоценный камень, если нет False
		/// </summary>
		internal bool IsMatched
        {
            get { return m_isMatched; }
            set { m_isMatched = value; }
        }

		/// <summary>
		/// Типы камня для обнаружения совпадения
		/// </summary>
		internal GemType TypeOfGem
        {
            get { return m_gemType; }
        }


		#endregion

		#region Builtin Methods

		private void Start()
		{
			m_currentGem = this;
		}

		private void Update()
		{
			// Остановим перемещение Функции лерп, чтобы избавитьсч от бесконечного перемещения
			if (Vector2.Distance(transform.position, m_currentPos) > 0.01f)
			{
				transform.position = Vector2.Lerp(transform.position, m_currentPos, m_boardOfGems.GemMovementSpeed * Time.deltaTime);
			}
			else
			{
				transform.position = new Vector2(m_currentPos.x, m_currentPos.y);
				m_boardOfGems.AllGems[m_currentPos.x, m_currentPos.y] = m_currentGem;
			}

			// Получить Финальное прикосновение пользователя
			if (m_isMouseButtonPressed && Input.GetMouseButtonUp(0))
			{
				m_isMouseButtonPressed = false; // Сразу говорим чтобы кнопка не была нажата, для контроля потока

				// убедимся что нам разрешено двигать изумруды на доске и время на уровне не равно нулю
				if (m_boardOfGems.CurrentBoardState == BoardState.Dynamic && m_boardOfGems.LevelManager.TimeOnTheRound > 0f)
                {
					m_lastTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); // извлечем позицию мыши на Экране 
					CalculateAngle();
				}

			}


		}

		#endregion

		#region CustomMethods

		internal void SetupGem(Vector2Int _pos, Board _board)
		{
			m_currentPos = _pos;
			m_boardOfGems = _board;
		}

		// Получить первое прикосновение пользователя
		private void OnMouseDown()
		{
			// Убедимся Если доска не в состоянии ожидания  и время на у уровне не ноль
			if(m_boardOfGems.CurrentBoardState == BoardState.Dynamic && m_boardOfGems.LevelManager.TimeOnTheRound > 0f)
            {
				m_firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				Debug.Log($"First Touch: { m_firstTouchPosition}.");
				// убедимся что кнопка мыши была нажата
				m_isMouseButtonPressed = true;

			}
		}

		// Разчитаем угол свайпа от начального прикосновения до финального 
		private void CalculateAngle()
		{
			m_swipeAngle = Mathf.Atan2(m_lastTouchPosition.y - m_firstTouchPosition.y, m_lastTouchPosition.x - m_firstTouchPosition.x);
			// конвертируем угол свайпа из радиан в градусы
			m_swipeAngle = m_swipeAngle * 180 / Mathf.PI;
			Debug.Log($"Swipe Angle: { m_swipeAngle}.");

			//Вектор3.Дистанция позволяет определить растояние между двумя векторами
			if (Vector3.Distance(m_firstTouchPosition, m_lastTouchPosition) > 0.5f)
			{
				MovePieces();
			}
		}

		private void MovePieces()
		{

			

			// Прежде чем переместить обьект, предидущая позиция этого камня будет равна текущей позиции в данный момент
			m_previousePos = m_currentPos;

			// Свайп   ВПРАВО   если диапазон угла составляет от -45 до 45
			// Поменяйте позицию кристалов
			if (m_swipeAngle < 45 && m_swipeAngle > -45)
			{
				// Если позиция текущего кристала по Х меньше чем ширина доски 
				if (m_currentGem.m_currentPos.x < m_boardOfGems.Width - 1)
				{
					// Получите целевой кристал который у которого нужно сменить позицию справа от первого касания 
					m_targetGem = m_boardOfGems.AllGems[m_currentPos.x + 1, m_currentPos.y];
					// Смените позицию по X целевого кристала, используя посфиксный декримент
					m_targetGem.m_currentPos.x--;
					// Смените позицию по X Текущего кристала, используя посфиксный инкримент
					m_currentGem.m_currentPos.x++;

					UpdateBackgroundArray();

				}
			}
			// Свайп  ВВЕРХ  если диапазон угла составляет от 45 до 135
			else if (m_swipeAngle > 45 && m_swipeAngle <= 135)
			{
				// Если позиция текущего кристала по Y меньше чем Высота доски 
				if (m_currentGem.m_currentPos.y < m_boardOfGems.Height - 1)
				{
					// Получите целевой кристал который у которого нужно сменить позицию верх от первого касания 
					m_targetGem = m_boardOfGems.AllGems[m_currentPos.x, m_currentPos.y + 1];
					// Смените позицию по Y целевого кристала, используя посфиксный декримент
					m_targetGem.m_currentPos.y--;
					// Смените позицию по Y Текущего кристала, используя посфиксный инкримент
					m_currentGem.m_currentPos.y++;

					UpdateBackgroundArray();
				}
			}
			// Свайп  ВНИЗ  если диапазон угла составляет от 45 до 135
			else if (m_swipeAngle < -45 && m_swipeAngle >= -135)
			{
				// Если позиция текущего кристала по Y больше чем нижняя грань доски 
				if (m_currentGem.m_currentPos.y > 0)
				{
					// Получите целевой кристал который у которого нужно сменить позицию вниз от первого касания 
					m_targetGem = m_boardOfGems.AllGems[m_currentPos.x, m_currentPos.y - 1];
					// Смените позицию по Y у целевого кристала, используя посфиксный декримент
					m_targetGem.m_currentPos.y++;
					// Смените позицию по Y Текущего кристала, используя посфиксный инкримент
					m_currentGem.m_currentPos.y--;

					//обновим Массив 
					UpdateBackgroundArray();
				}

			}
			// Свайп  ВЛЕВО  если диапазон угла составляет от -45 до 135
			else if (m_swipeAngle > 135 && m_swipeAngle > -135)
			{
				// Если позиция текущего кристала по X больше чем Левая грань доски 
				if (m_currentGem.m_currentPos.x > 0)
				{
					// Получите целевой кристал который у которого нужно сменить позицию вниз от первого касания 
					m_targetGem = m_boardOfGems.AllGems[m_currentPos.x - 1, m_currentPos.y];
					// Смените позицию по X у целевого кристала, используя посфиксный декримент
					m_targetGem.m_currentPos.x++;
					// Смените позицию по X Текущего кристала, используя посфиксный инкримент
					m_currentGem.m_currentPos.x--;

					UpdateBackgroundArray();

				}
			}

			// для предотвращения ошибки, когда игра зависает, иза свайпа за пределы доски
			if (m_targetGem == null)
			{
				return;
			}


			// Переместите камень в нужную позицию, а затем выполните проверку, должен ли он остаться в полученой позии
			// затем верните камень на предидущую позицию если камень не совпал
			StartCoroutine(CheckMove_Corutine());
		}

		private void UpdateBackgroundArray()
		{
			m_boardOfGems.AllGems[m_currentPos.x, m_currentPos.y] = m_currentGem;
			m_boardOfGems.AllGems[m_targetGem.m_currentPos.x, m_targetGem.m_currentPos.y] = m_targetGem;
		}
		
		/// <summary>
		/// Корутина позволяет изменить позицию изумрудов, а также уничтожить их в случае совпадения
		/// </summary>
		/// <returns></returns>
		private IEnumerator CheckMove_Corutine()
        {
			// во время проверки движения изумрудов. Установим состояние доски вврежим ожидания, чтобы игрок не производил действия
			m_boardOfGems.CurrentBoardState = BoardState.Static;

			yield return new WaitForSeconds(0.5f);

			// Перепроверим все ли совпадения были проверены на этом этапе
			m_boardOfGems.FinderOfMatch.FindAllMatches();

			// убедимся в том что перемещенный Камень в настоящее время не разрушен и существует во время перемещения
			if (m_targetGem != null)
            {
				// Убедимся, что любой из драгоценных камней, которые мы только что переместили, не совпадают.  
				if (!m_currentGem.m_isMatched & !m_targetGem.m_isMatched)
                {
					// Итак, если ни один из них не совпадает, что ж, тогда нам нужно вернуться к нашим предыдущим позициям.
					m_targetGem.m_currentPos = m_currentPos;
					m_currentGem.m_currentPos = m_previousePos;

					// Обновим позиции всех камней на доске
					UpdateBackgroundArray();

					// подождем чтобы вернулисьвсе изумруды в их позиции 
					yield return new WaitForSeconds(0.5f);
					// и позволим игроку обновлять доску
					m_boardOfGems.CurrentBoardState = BoardState.Dynamic;
				}
				else // иначе если совпадает, уничтожь драгоценные камни в списка которые совпадают
				{
					m_boardOfGems.DestroyMatches();
                }
			}


		}
		#endregion

	}

}