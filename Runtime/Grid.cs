using System;
using System.Collections.Generic;
using UnityEngine;

namespace LeonDrace.Grid
{
	public class Grid<TCell>
	{
		public event Action<TCell> OnAnyValueChanged;

		private readonly int m_Width;
		private readonly int m_Height;
		private readonly int m_CellSize;
		private readonly Vector3 m_OriginPosition;
		private TCell[,] m_Cells;

		public Grid(Vector3 originPosition, GridSize size)
		{
			m_Width = size.width;
			m_Height = size.height;
			m_CellSize = size.cellSize;
			m_OriginPosition = originPosition;

			m_Cells = new TCell[m_Width, m_Height];

			if (typeof(TCell).IsClass)
			{
				for (int x = 0; x < m_Cells.GetLength(0); x++)
				{
					for (int y = 0; y < m_Cells.GetLength(1); y++)
					{
						m_Cells[x, y] = Activator.CreateInstance<TCell>();
					}
				}
			}
		}

		/// <summary>
		/// Get the cells world position.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public Vector3 GetWorldPosition(int x, int y)
		{
			return new Vector3(x, y) * m_CellSize + m_OriginPosition;
		}

		/// <summary>
		/// Get the cells centered world position.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public Vector3 GetWorldPositionCenter(int x, int y)
		{
			return GetWorldPosition(x, y) + Vector3.one * (m_CellSize / 2f);
		}

		/// <summary>
		/// Get the cell indecies from the world position.
		/// </summary>
		/// <param name="worldPosition"></param>
		/// <returns></returns>
		public (int x, int y) GetCellFromWorldPosition(Vector3 worldPosition)
		{
			int x = Mathf.FloorToInt((worldPosition - m_OriginPosition).x / m_CellSize);
			int y = Mathf.FloorToInt((worldPosition - m_OriginPosition).y / m_CellSize);

			return (x, y);
		}

		/// <summary>
		/// Set the value for cell with indecies.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="value"></param>
		public void SetValue(int x, int y, TCell value)
		{
			if (!IsValid(x, y)) return;

			m_Cells[x, y] = value;
			OnAnyValueChanged?.Invoke(value);
		}

		/// <summary>
		/// Set the value for cell at world position.
		/// </summary>
		/// <param name="worldPosition"></param>
		/// <param name="value"></param>
		public void SetValue(Vector3 worldPosition, TCell value)
		{
			var cellIndecies = GetCellFromWorldPosition(worldPosition);
			SetValue(cellIndecies.x, cellIndecies.y, value);
		}

		/// <summary>
		/// Get all cells as jagged array.
		/// </summary>
		/// <returns></returns>
		public TCell[,] GetCells()
		{
			return m_Cells;
		}

		/// <summary>
		/// Get all cells as list.
		/// </summary>
		/// <returns></returns>
		public List<TCell> GetCellsAsList()
		{
			List<TCell> cells = new();

			for (int x = 0; x < m_Cells.GetLength(0); x++)
			{
				for (int y = 0; y < m_Cells.GetLength(1); y++)
				{
					cells.Add(m_Cells[x, y]);
				}
			}

			return cells;
		}

		/// <summary>
		/// Get the cell with indecies.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns>Will return default if not valid.</returns>
		public TCell GetValueAt(int x, int y)
		{
			if (!IsValid(x, y)) return default;

			return m_Cells[x, y];
		}

		/// <summary>
		/// Get the cell at world position.
		/// </summary>
		/// <param name="worldPosition"></param>
		/// <returns></returns>
		public TCell GetValueAt(Vector3 worldPosition)
		{
			var cell = GetCellFromWorldPosition(worldPosition);
			return GetValueAt(cell.x, cell.y);
		}

		private bool IsValid(int x, int y)
		{
			return x >= 0 && y >= 0 && x < m_Width && y < m_Height;
		}
	}

	[System.Serializable]
	public struct GridSize
	{
		public int width;
		public int height;
		public int cellSize;
	}
}
