using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour {
    public SpriteRenderer blue, green, purple, red, yellow;
    public int xSiz, ySiz;
    public GameObject[] gems;
    public GameObject boombs;
    private BoardItem[,] _items;
    private BoardItem _currentlySelectedItem;
    public static int minItemsForMatch = 3;
    public float delayBetweenMatches = 0.2f;
    public bool canPlay;
    // Use this for initialization
    void Start() {
        GetGems();
        SizeBoard();
        ClearBoard();
        BoardItem.OnMouseOverItemEventHandler += OnMouseOverItem;
        List<BoardItem> matchesForItem = SearchHorizontally(_items [3,3]);
        if (matchesForItem.Count >= 3) { Debug.Log("На клетке 3.3 есть пара"); }
        else { Debug.Log("На клетке 3.3 нет пара"); }
    }
    void Update()
    {
        
    }

        void OnDisable()
    {
        BoardItem.OnMouseOverItemEventHandler -= OnMouseOverItem;
    }

    void SizeBoard()
    {
        _items = new BoardItem[xSiz, ySiz];
        for (int x = 0; x < xSiz; x++)
        {
            for (int y = 0; y < ySiz; y++)
            {
                _items[x, y] = InstantiateGem(x, y);
            }
        }
    }

    void ClearBoard()
    {
        for (int x = 0; x < xSiz; x++)
        {
            for (int y = 0; y <ySiz; y++)
            {
                MatchInfo matchInfo = GetMatchInformation(_items [x, y]);
                if (matchInfo.ValidMatch)
                {
                    Destroy(_items [x, y].gameObject);
                    _items[x, y] = InstantiateGem(x, y);
                    y--;
                }
            }
        }
    }

    BoardItem InstantiateGem(int x, int y)
    {
        GameObject randomGem = gems[Random.Range(0, gems.Length)];
        BoardItem newGem = ((GameObject)Instantiate(randomGem, new Vector3(x, y), Quaternion.identity)).GetComponent<BoardItem>();
        newGem.OnItemPositionChanged(x, y);
        //newGem.GetComponent<BoardItem>().OnItemPositionChanged(x, y);
        return newGem;
    }

    void OnMouseOverItem(BoardItem item)
    {
        if (_currentlySelectedItem == item && canPlay) // если 2 клик сделан по тому же объекту что и в 1 раз
        {
            //  _currentlySelectedItem.GetComponent<Animator>().enabled = false; 
            //  _currentlySelectedItem.transform.rotation = new Quaternion(0, 0, 0, 0);
            //    item.GetComponent<Animator>().enabled = false; // выключает анимацию
            //    item.transform.rotation = new Quaternion(0, 0, 0, 0); // задает изначальный вид
            _currentlySelectedItem = null;// обнуляет сохраненый клик
                        canPlay = false;
            return; // возвращает null методу 
        }
        if (_currentlySelectedItem == null && !canPlay) // если в сохраненном клике ничего нет 
        {
         //   item.GetComponent<Animator>().enabled = true; //включение анимации
           // item.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
            _currentlySelectedItem = item; // сохраняет значение клика
            canPlay = true;
        }
        else //тогда, клик по другому не равному объекту
        {
            float xDiff = Mathf.Abs(item.x - _currentlySelectedItem.x);// формула расчета приделов клика по горизонтале 
            float yDiff = Mathf.Abs(item.y - _currentlySelectedItem.y);// формула расчета приделов клика по вертикале
            if (xDiff + yDiff == 1) // если придели клика не больше допустимого
            {
                StartCoroutine(TryMatch (_currentlySelectedItem, item));// вызов метода перемещения гемов в которые вносятся данные 2 кликов
                canPlay = false; 
                //    _currentlySelectedItem.GetComponent<Animator>().enabled = false;// выключение анимации 
                //    _currentlySelectedItem.transform.rotation = new Quaternion(0, 0, 0, 0);// задает изначальный вид
                _currentlySelectedItem =  null;
                
            }
            else // если клик сделан больше зоны допустимого
            {
                
                _currentlySelectedItem = null;
                canPlay = false;
                
            }
          //  _currentlySelectedItem.GetComponent<Animator>().enabled = false;//выключение анимации 
          //  _currentlySelectedItem.transform.rotation = new Quaternion(0, 0, 0, 0);//задает изначальный вид
            // обнуляет сохраненый клик
        }
    }
    IEnumerator TryMatch (BoardItem a, BoardItem b)
    {
        
        yield return StartCoroutine(Swap(a, b));
        MatchInfo matchA = GetMatchInformation(a);
        MatchInfo matchB = GetMatchInformation(b);
        if (!matchA.ValidMatch && !matchB.ValidMatch)
        {
            yield return StartCoroutine(Swap (a,b));
            
            yield break;
        }
        if (matchA.ValidMatch)
        {
            if (matchA.match.Count > 3)
            {
                
                
                yield return StartCoroutine(DestroyItems(matchA.match));
                yield return new WaitForSeconds(delayBetweenMatches);
                //Debug.Log(matchA.matchEndingX);
                yield return StartCoroutine(UpdateBoardAfterMetch(matchA));
               // Debug.Log(a.name);

                switch (a.id)
                {
                    case 0:
                        a.GetComponent<SpriteRenderer>().sprite = blue.sprite;
                        break;
                    case 1:
                        a.GetComponent<SpriteRenderer>().sprite = green.sprite;
                        break;
                    case 2:
                        a.GetComponent<SpriteRenderer>().sprite = purple.sprite;
                        break;
                    case 3:
                        a.GetComponent<SpriteRenderer>().sprite = red.sprite;
                        break;
                    case 4:

                        a.GetComponent<SpriteRenderer>().sprite = yellow.sprite;
                        break;
                    default:
                        break;
                }
                //((GameObject)Instantiate(randomGem, new Vector3(x, y), Quaternion.identity)).GetComponent<BoardItem>();
                // BoardItem newbomb = ((GameObject)Instantiate(boombs, new Vector3(matchA.matchEndingX, matchA.matchEndingY), Quaternion.identity)).GetComponent<BoardItem>();
                //newbomb.id = a.id;
                // newbomb.OnItemPositionChanged(matchA.matchEndingX, matchA.matchEndingY);

                //yield return StartCoroutine(UpdateBoardAfterMetch(matchA));
            }
            yield return StartCoroutine (DestroyItems (matchA.match));
            yield return new WaitForSeconds(delayBetweenMatches);
            yield return StartCoroutine (UpdateBoardAfterMetch (matchA));
        }
        else if (matchB.ValidMatch)
        {
            yield return StartCoroutine (DestroyItems (matchB.match));
            yield return new WaitForSeconds(delayBetweenMatches);
            yield return StartCoroutine (UpdateBoardAfterMetch (matchB));
        }
        
    }

    IEnumerator UpdateBoardAfterMetch(MatchInfo match)
    {
        
        if (match.matchStartingY == match.matchEndingY)
        {
            for (int x = match.matchStartingX; x <= match.matchEndingX; x++)
            {
                for (int y = match.matchStartingY; y < ySiz - 1; y++)
                {
                    BoardItem upperIndex = _items[x, y + 1];
                    BoardItem current = _items[x, y];
                    _items[x, y] = upperIndex;
                    _items[x, y + 1] = current;
                    _items[x, y].OnItemPositionChanged(_items[x, y].x, _items[x, y].y - 1);
                }
                _items [x, ySiz -1] = InstantiateGem(x, ySiz - 1);
            }
        }
        else if (match.matchEndingX == match.matchStartingX)
        {
            Debug.Log(match);
            int matchHeight = 1 + (match.matchEndingY - match.matchStartingY);
            for (int y = match.matchStartingY + matchHeight; y <= ySiz - 1; y++)
            {
                BoardItem lowerIndex = _items[match.matchStartingX, y - matchHeight];
                BoardItem current = _items[match.matchStartingX, y];
                _items[match.matchStartingX, y - matchHeight] = current;
                _items[match.matchStartingX, y] = lowerIndex;
            }
            for (int y = 0; y < ySiz - matchHeight; y++)
            {
                _items[match.matchStartingX, y].OnItemPositionChanged(match.matchStartingX, y);
            }
            for (int i = 0; i < match.match.Count; i++)
            {
                _items[match.matchStartingX, (ySiz - 1) - i] = InstantiateGem(match.matchStartingX, (ySiz - 1) - i);
            }
        }

        for (int x = 0; x < xSiz; x++)
        {
            for (int y = 0; y < ySiz; y++)
            {
                MatchInfo matchInfo = GetMatchInformation(_items[x, y]);
                if (matchInfo.ValidMatch)
                {
                    yield return new WaitForSeconds (delayBetweenMatches);
                    yield return StartCoroutine(DestroyItems (matchInfo.match));
                    yield return new WaitForSeconds(delayBetweenMatches);
                    yield return StartCoroutine(UpdateBoardAfterMetch(matchInfo));
                }
            }
        }
    }


    IEnumerator DestroyItems(List<BoardItem> items)
    {
       foreach(BoardItem i in items)
       {
           yield return StartCoroutine(i.transform.Scale(Vector3.zero, 0.5f));
           Destroy(i.gameObject);

       }
       
    }

    IEnumerator Swap (BoardItem a, BoardItem b)
    {
        ChangeRigidbodeStatus(false);
        float movDuration = 0.1f;
        Vector3 aPosition = a.transform.position;
        StartCoroutine(a.transform.Move(b.transform.position, movDuration));
        StartCoroutine(b.transform.Move(aPosition, movDuration));
        yield return new WaitForSeconds (movDuration);
        SwapIndiece(a,b);
        ChangeRigidbodeStatus(true);
    }
    void SwapIndiece(BoardItem a, BoardItem b) //изменение имени спрайта, путем изменения имени координат, 1 клик(на спрайт) именует ником второго клика, второго клика соответственно именем 1
    {
        BoardItem tempA = _items[a.x, a.y];
        _items [a.x, a.y] = b;
        _items[b.x, b.y] = tempA;
        int bOldX = b.x; int bOldY = b.y;
        b.OnItemPositionChanged(a.x, a.y);
        a.OnItemPositionChanged(bOldX, bOldY);
    }

    List<BoardItem> SearchHorizontally(BoardItem item)
    {
        List<BoardItem> hItems = new List<BoardItem> { item };
        int left = item.x - 1;
        int right = item.x + 1;
        while (left >= 0 && _items[left, item.y] != null && _items[left, item.y].id == item.id)
        {
            hItems.Add(_items[left, item.y]);
            left--;
        }
        while (right < xSiz && _items[right, item.y] != null && _items[right, item.y].id == item.id)
        {
            hItems.Add(_items[right, item.y]);
            right++;
        }
        return hItems;
    }
    List<BoardItem> SearchVertically(BoardItem item)
    {
        List<BoardItem> vItems = new List<BoardItem> { item };
        int lower = item.y - 1;
        int downer = item.y + 1;
        while (lower >= 0 && _items[item.x, lower] != null && _items[item.x, lower].id == item.id)
        {
            vItems.Add(_items[item.x, lower ]);
            lower--;
        }
        while (downer < ySiz && _items[item.x, downer] != null && _items [item.x, downer].id == item.id)
        {
            vItems.Add(_items[item.x, downer]);
            downer++;
        }
        return vItems;
    }
    MatchInfo GetMatchInformation(BoardItem item)
    {
        MatchInfo m = new MatchInfo();
        m.match = null;
        List<BoardItem> hMatch = SearchHorizontally(item);
        List<BoardItem> vMatch = SearchVertically(item);

        if (hMatch.Count >= minItemsForMatch && hMatch.Count > vMatch.Count)
        {
           // Debug.Log(hMatch.Count); // фывапролджжэйцукенгшщзхячсмитьбю.ясывымфыафиаишгпаипвщтвыпщвытвмытшвымвышмгтвмышгтмвышгтвмышгмывтшвмытшмвыгтвмыгшвмышг
            m.matchStartingX = GetMinimumX(hMatch);
            m.matchEndingX = GetMaximumX(hMatch);
            m.matchStartingY = m.matchEndingY = hMatch[0].y;
            m.match = hMatch;
        }
        else if (vMatch.Count >= minItemsForMatch)
        {
            m.matchStartingY = GetMinimumY(vMatch);
            m.matchEndingY = GetMaximumY(vMatch);
            m.matchStartingX = m.matchEndingX = vMatch[0].x;
            m.match = vMatch;
        }
        return m;
    }

    int GetMinimumX(List<BoardItem> items)
    {
        float[] indices = new float[items.Count];
        for (int i = 0; i < indices.Length; i++)
        {
            indices[i] = items[i].x;
        }
        return (int)Mathf.Min(indices);
    }

    int GetMaximumX(List<BoardItem> items)
    {
        float[] indices = new float[items.Count];
        for (int i = 0; i < indices.Length; i++)
        {
            indices[i] = items[i].x;
        }
        return (int)Mathf.Max(indices);
    }

    int GetMinimumY(List<BoardItem> items)
    {
        float[] indices = new float[items.Count];
        for (int i = 0; i < indices.Length; i++)
        {
            indices[i] = items[i].y;
        }
        return (int)Mathf.Min(indices);
    }

    int GetMaximumY(List<BoardItem> items)
    {
        float[] indices = new float[items.Count];
        for (int i = 0; i < indices.Length; i++)
        {
            indices[i] = items[i].y;
        }
        return (int)Mathf.Max(indices);
    }

    void GetGems()
    {
        for (int i =0; i< gems.Length; i++)
        {
            gems[i].GetComponent<BoardItem>().id = i;
        }
    }
    void ChangeRigidbodeStatus(bool status)
    {
        foreach(BoardItem g in _items)
        {
            if (g != null)
            {
                g.GetComponent<Rigidbody2D>().isKinematic = !status;
            }
        }
    }
}