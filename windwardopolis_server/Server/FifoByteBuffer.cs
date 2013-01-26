// code primarily from http://stackoverflow.com/questions/7122972/buffering-byte-data-in-c
// Edited by Windward Studios, Inc. (www.windward.net). No copyright claimed by Windward on changes.

using System;
using System.Collections;

namespace Windwardopolis
{
	public class FifoByteBuffer
	{
		// bugbug - auto-increase the buffer size if needed (client too).
		private byte[] _buffer;
		private int _endIndex;
		private int _startIndex;

		public FifoByteBuffer(int capacity)
		{
			_buffer = new byte[capacity];
		}

		public int Count
		{
			get
			{
				if (_endIndex > _startIndex)
					return _endIndex - _startIndex;
				if (_endIndex < _startIndex)
					return (_buffer.Length - _startIndex) + _endIndex;
				return 0;
			}
		}

		public byte this[int index]
		{
			get
			{
				if (index >= Count) 
					throw new ArgumentOutOfRangeException();
				return _buffer[(_startIndex + index) % _buffer.Length];
			}
		}

		public IEnumerable Bytes
		{
			get
			{
				for (int i = _startIndex; i < Count; i++)
					yield return _buffer[(_startIndex + i)%_buffer.Length];
			}
		}

		public void Grow(int minLength)
		{
			if (minLength < _buffer.Length)
				return;

			int newLen = _buffer.Length;
			while (minLength >= newLen)
				newLen *= 2;
			// the size of a message can be of any length because of the avatar in the join. So we grow the buffer to fit.
			// we only need to create new & copy if the buffer has wrapped - but safer to have one code path
			byte[] data = _Read(Count, false);
			_buffer = new byte[newLen];
			Array.Copy(data, _buffer, data.Length);
			_startIndex = 0;
			_endIndex = data.Length;
		}

		public void Write(byte[] data)
		{
			Write(data, 0, data.Length);
		}

		public void Write(byte[] data, int offset, int length)
		{

			if (Count + length >= _buffer.Length)
				Grow(Count + length);

			if (_endIndex + length >= _buffer.Length)
			{
				int endLen = _buffer.Length - _endIndex;
				int remainingLen = length - endLen;

				Array.Copy(data, offset, _buffer, _endIndex, endLen);
				Array.Copy(data, offset + endLen, _buffer, 0, remainingLen);
				_endIndex = remainingLen;
			}
			else
			{
				Array.Copy(data, offset, _buffer, _endIndex, length);
				_endIndex += length;
			}
		}

		public byte[] Read(int len)
		{
			return _Read(len, false);
		}

		public byte[] Peek(int len)
		{
			return _Read(len, true);
		}

		private byte[] _Read(int len, bool keepData)
		{
			if (len > Count) 
				throw new Exception("not enough data in buffer");

			var result = new byte[len];
			if (_startIndex + len < _buffer.Length)
			{
				Array.Copy(_buffer, _startIndex, result, 0, len);
				if (!keepData)
					_startIndex += len;
				return result;
			}
			int endLen = _buffer.Length - _startIndex;
			int remainingLen = len - endLen;
			Array.Copy(_buffer, _startIndex, result, 0, endLen);
			Array.Copy(_buffer, 0, result, endLen, remainingLen);
			if (!keepData)
				_startIndex = remainingLen;
			return result;
		}
	
	}
}