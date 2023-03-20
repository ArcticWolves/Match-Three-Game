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
    public class TestBoard : MonoBehaviour
    {
        [Header("Board Properties")]

        [SerializeField]
        private int m_width = 0;

        [SerializeField]
        private int m_height = 0;


        [SerializeField]
        private GameObject[] m_backgroundTilePrefab = null;



        [SerializeField]
        private GameObject[] m_gems = null;

        // двухмерный масив для хранения значений позицый X и Y обьектов кристала
        [SerializeField]
        private GameObject[,] m_allGems = null;

    

        private void Start()
        {
            // Создание обьектов в количестве по ширине и высоте
            m_allGems = new GameObject[m_width, m_height];
            


            if (m_backgroundTilePrefab != null & m_gems != null)
            {
                BuildObjOnCell(m_backgroundTilePrefab, m_allGems, m_width, m_height, "Board", _isRandomVariety:false);
                BuildObjOnCell(m_gems, m_allGems, m_width, m_height, "Gems" , _isRandomVariety:true);

            }


        }

        

      
        public void BuildObjOnCell(GameObject[] _objectsToBuild, GameObject[,] _arrayPosObjects, int _width, int _height, string _nameOfObjects, bool _isRandomVariety = false)
        {
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    Vector2Int _pos = new Vector2Int(x, y);
                    int _indexOfRange = 0;
                    if (_isRandomVariety)
                    {
                        _indexOfRange = Random.Range(0, _objectsToBuild.Length);
                    }

                    SpawnObject(_objectsToBuild[_indexOfRange], _pos, _arrayPosObjects, _nameOfObjects);

                }
            }

        }

       





        /// <summary>
        /// Создаёт обьект с именем и позицией
        /// </summary>
        /// <param name="_objToSpawn"> Какой обьект будет спавниться</param>
        /// <param name="_posToSpawn"> Позиция спавна X и Y кординат </param>
        /// <param name="_nameOfObject"> Название обьекта </param>
        public void SpawnObject(GameObject _objectToSpawn, Vector2Int _posToSpawn, GameObject[,] _allObjPosArray, string _nameOfObject)
        {
            // Локальная Конвертация полученого 2D Вектора в 3D вектор
            Vector3 _vec3PosSpawn = new Vector3(_posToSpawn.x, _posToSpawn.y, 0f);

            GameObject _objInstantiate = Instantiate(_objectToSpawn, _vec3PosSpawn, Quaternion.identity);
            _objInstantiate.transform.parent = this.transform;
            _objInstantiate.name = $"{_nameOfObject} - {_posToSpawn.x}, {_posToSpawn.y}";

            // Двухмерный массив каждого обьекта для хранения позиции X и Y каждого обьекта
            _allObjPosArray[_posToSpawn.x, _posToSpawn.y] = _objInstantiate;

            // Установим позицию 
            _objInstantiate.transform.position = new Vector2(_posToSpawn.x, _posToSpawn.y);
         
            

        }

        public void SpawnObject()
        {

        }

    }

}