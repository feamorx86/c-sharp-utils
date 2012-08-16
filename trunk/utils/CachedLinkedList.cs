using System;

public class CachedLinkedListNode<T> 
{
	public CachedLinkedListNode(T value) 
        {
			_value = value;
			_next = null;
			_previous = null;
		}

	public CachedLinkedListNode() 
        {
			_next = null;
			_previous = null;
			_value = default(T);
		}

		T _value;

		CachedLinkedListNode<T> _next;
		CachedLinkedListNode<T> _previous;

	public CachedLinkedListNode<T> Next 
        {
			get { return _next; }
			set { _next = value; }
		}

	public CachedLinkedListNode<T> Previous 
        {
			get { return _previous; }
			set { _previous = value; }
		}

	public T Value 
        {
			get { return _value; }
			set { _value = value; }
		}
	}

	public class LinkedListCach<T> 
    {
		CachedLinkedListNode<T> _first;
		int _count;
		int _MaximumSize;

		public const int DefaultMaximumSize=256;

		public int Count 
        {
			get { return _count; }
			set 
            {
				Resize(value);
			}
		}

		public int MaximumSize 
        {
			get { return _MaximumSize; }
			set 
            {
				_MaximumSize = value;
				if (_count > _MaximumSize)
					Resize(_MaximumSize);
			}
		}

		public void Resize(int newSize) 
        {
			if (newSize == Count)
				return;
			if (newSize == 0) 
            { 
                Clear();
                return; 
            }
			if (newSize > _MaximumSize)
				newSize = _MaximumSize;
			if (newSize > Count) 
            {
				for (int i = newSize - Count; i > 0; --i) 
                {
					AddToCach(new CachedLinkedListNode<T>());
				}
			}
            else 
            {
				for (int i = Count - newSize; i > 0; --i) 
                {
					RemoveFirst();
				}
			}
			_count = newSize;
		}

		public void AddToCach(CachedLinkedListNode<T> value) 
        {
			if (_count >= _MaximumSize)
				return;
			if (_first == null) 
            {
				_first = value;
				_first.Next = null;
				_first.Previous = null;
				_first.Value = default(T);
			} 
            else 
            {
				_first.Previous = value;
				value.Next = _first;
				value.Previous = null;
				value.Value = default(T);
				_first = value;
			}
			++_count;
		}

        public void AddNodesToCach(CachedLinkedListNode<T> StartNode, CachedLinkedListNode<T> EndNode)
        {
            if (_count == _MaximumSize)
                return;           
            {
                
                if (_first == null)
                {
                    _first = StartNode;
                    StartNode.Previous = null;
                    EndNode.Next = null;
                }
                else
                {
                    _first.Previous = EndNode;
                    EndNode.Next = _first;
                    StartNode.Previous = null;
                    _first = StartNode;
                }

                CachedLinkedListNode<T> i = EndNode;
                //zero values
                while (i != null)
                {
                    if (_count >= _MaximumSize)
                    {
                        _first = i;
                        i = i.Previous;
                        _first.Previous = null;

                        if (i != null)
                        {
                            while (i.Previous != null)
                            {
                                i.Value = default(T);
                                i.Previous.Next = null;
                                i = i.Previous;
                            }
                        }
                        return;
                    }
                    i.Value = default(T);
                    i = i.Previous;
                    ++_count;
                }
            }
        }


		public CachedLinkedListNode<T> GetFromCach() 
        {
			if (_count > 0) 
            {
				CachedLinkedListNode<T> r = _first;
				RemoveFirst();
				return r;
			}
            else
				return new CachedLinkedListNode<T>();
		}

		public CachedLinkedListNode<T> GetFromCach(T value) 
        {
			if (_count > 0) 
            {
				CachedLinkedListNode<T> r = _first;
				RemoveFirst();
				r.Value = value;
				return r;
			}
            else
				return new CachedLinkedListNode<T>(value);
		}

		public void Clear() 
        {
			if (_first == null)
				return;
			CachedLinkedListNode<T> i = _first,j;

		Cont:
			j = i.Next;
			i.Next = null;
			i.Previous = null;
			i.Value = default(T);

			if (j == null) 
            {
				goto Final;
			}
            else
            {
				i = j;
				goto Cont;
			}
		Final:
			_first = null;
			_count = 0;
		}

		public void RemoveFirst() 
        {
			if (_first != null)
            {
				CachedLinkedListNode<T> i = _first.Next;
				_first.Next = null;
				_first = i;
				if (_first != null)
					_first.Previous = null;
				--_count;
			}
		}

		public LinkedListCach() 
        {
			//_first = new CachedLinkedListNode<T>();
			_MaximumSize = DefaultMaximumSize;
			_count = 0;
		}

		public LinkedListCach(int Size) 
        {
			_MaximumSize = DefaultMaximumSize;
			Resize(Size);
		}

		public LinkedListCach(int Size, int MaxSize) 
        {
			_MaximumSize = MaxSize;
			Resize(Size);
		}

		~LinkedListCach() 
        {
			Clear();
		}
	}

	public class CachedLinkedList<T> 
    {

		LinkedListCach<T> _cach;
		CachedLinkedListNode<T> _first;
		CachedLinkedListNode<T> _last;

		int _count;

		public CachedLinkedList(LinkedListCach<T> cach) 
        {
			_cach = cach;
			_first = null;
			_last = null;
			_count = 0;
		}

        ~CachedLinkedList()
        {
            Clear();
        }

		public int Count 
        {
			get { return _count; }
		}

		public CachedLinkedListNode<T> First 
        {
			get { return _first; }
			set { _last = value; }
		}

		public CachedLinkedListNode<T> Last 
        {
			get { return _last; }
			set { _last = value; }
		}

		public LinkedListCach<T> Cach 
        {
			get { return _cach; }
			set { _cach = value; }
		}

		public void AddAfter(CachedLinkedListNode<T> node, CachedLinkedListNode<T> newNode) 
        {
			if (node == Last) 
            {
				node.Next = newNode;
				newNode.Previous = node;
				newNode.Next = null;
				Last = newNode;
			} 
            else 
            {
				CachedLinkedListNode<T> nextNode = node.Next;
				node.Next = newNode;
				newNode.Previous = node;

				nextNode.Previous = newNode;
				newNode.Next = nextNode;
			}
			++_count;
		}

		public CachedLinkedListNode<T> AddAfter(CachedLinkedListNode<T> node, T value)
        {
			CachedLinkedListNode<T> newNode = Cach.GetFromCach();
			newNode.Value = value;
			if (node == Last)
            {
				node.Next = newNode;
				newNode.Previous = node;
				newNode.Next = null;
				Last = newNode;
			} 
            else 
            {
				CachedLinkedListNode<T> nextNode = node.Next;
				node.Next = newNode;
				newNode.Previous = node;

				nextNode.Previous = newNode;
				newNode.Next = nextNode;
			}
			++_count;
			return newNode;
		}

		public void AddBefore(CachedLinkedListNode<T> node, CachedLinkedListNode<T> newNode) 
        {
			if (node == First) 
            {
				node.Previous = newNode;
				newNode.Previous = null;
				newNode.Next = node;
				First = newNode;
			} 
            else 
            {
				CachedLinkedListNode<T> PrevNode = node.Previous;
				node.Previous = newNode;
				newNode.Next = node;

				PrevNode.Next = newNode;
				newNode.Previous = PrevNode;
			}
			++_count;
		}

		public CachedLinkedListNode<T> AddBefore(CachedLinkedListNode<T> node, T value) 
        {
			CachedLinkedListNode<T> newNode = Cach.GetFromCach(value);
			if (node == First) 
            {
				node.Previous = newNode;
				newNode.Previous = null;
				newNode.Next = node;
				First = newNode;
			}
            else 
            {
				CachedLinkedListNode<T> PrevNode = node.Previous;
				node.Previous = newNode;
				newNode.Next = node;

				PrevNode.Next = newNode;
				newNode.Previous = PrevNode;
			}
			++_count;
			return newNode;
		}

		public void AddFirst(CachedLinkedListNode<T> node)
        {
			First.Previous = node;
			node.Previous = null;
			node.Next = First;
			First = node;
			++_count;
		}

		public CachedLinkedListNode<T> AddFirst(T value)
        {
			CachedLinkedListNode<T> node = Cach.GetFromCach(value);
			First.Previous = node;
			node.Previous = null;
			node.Next = First;
			First = node;
			++_count;
			return node;
		}

		public void AddLast(CachedLinkedListNode<T> node) 
        {
			Last.Next = node;
			node.Previous = Last;
			node.Next = null;
			Last = node;
			++_count;
		}

		public CachedLinkedListNode<T> AddLast(T value)
        {
			CachedLinkedListNode<T> node = Cach.GetFromCach(value);
			Last.Next = node;
			node.Previous = Last;
			node.Next = null;
			Last = node;
			++_count;
			return node;
		}

		public void Clear() 
        {
			if (_count == 0)
				return;
			CachedLinkedListNode<T> i = _first, j;

		Cont:
			j = i.Next;
			i.Next = null;
			i.Previous = null;
			i.Value = default(T);

			if (j == null) 
            {
				goto Final;
			}
            else
            {
				i = j;
				goto Cont;
			}
		Final:
			_first = null;
			_last = null;
			_count = 0;
		}

		public bool Contains(T value) 
        {
			CachedLinkedListNode<T> i = _first;
			while (i != null) 
            {
				if (i.Value.Equals(value))
					return true;
				i = i.Next;
			}
			return false;
		}

		public CachedLinkedListNode<T> Find(T value) 
        {
			CachedLinkedListNode<T> i = _first;
			while (i != null) 
            {
				if (i.Value.Equals(value))
					return i;
				i = i.Next;
			}
			return null;
		}

		public CachedLinkedListNode<T> FindLast(T value) 
        {
			CachedLinkedListNode<T> i = _last;
			while (i != null)
            {
				if (i.Value.Equals(value))
					return i;
				i = i.Previous;
			}
			return null;
		}

		public void Remove(CachedLinkedListNode<T> node) 
        {
			if (node == First)
            {
				First = First.Next;
				if (First != null)
					First.Previous = null;
				else
					Last = null;
			}
            else
				if (node == Last) 
                {
					Last = Last.Previous;
					if (Last != null)
						Last.Next = null;
					else
						First = null;
				} 
                else
                {
					node.Next.Previous = node.Previous;
					node.Previous.Next = node.Next;
				}
			Cach.AddToCach(node);
			--_count;
		}

		public bool Remove(T value)
        {
			if (Count == 0)
				return false;
			CachedLinkedListNode<T> node = Find(value);
			if (node == null)
				return false;
			Remove(node);
			return true;
		}

		public void RemoveFirst()
        {
			if (Count == 0)
				return;
			CachedLinkedListNode<T> i = First;
			First = First.Next;
			if (First != null)
				First.Previous = null;
			else
				Last = null;
			--_count;
			Cach.AddToCach(i);
		}

		public void RemoveLast()
        {
			if (Count == 0)
				return;
			CachedLinkedListNode<T> i = Last;
			Last = Last.Previous;
			if (Last != null)
				Last.Next = null;
			else
				First = null;
			--_count;
			Cach.AddToCach(i);
		}
	}
