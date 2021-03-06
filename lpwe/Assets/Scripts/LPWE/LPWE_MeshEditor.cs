﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LPWE_MeshEditor : LPWE_Component 
{


	public MeshTools current_used_tool;
	public FlatOptions flat_options;


	public float raise_force;
	public float flat_force;
	public float fr_force;
	public float s_height;
	public float random_range;


	public void SetHeight(SelectedVertices sv, float height)
	{
		Vector3[] vertices = main.chunks[sv.chunk_index].mesh_filter.sharedMesh.vertices;
	
		foreach(SV s in sv.vertices_index) 
		{
			if(s.serial_index % 3 == 0)
			{
				vertices [s.serial_index] = new Vector3(vertices[s.serial_index].x, height, vertices[s.serial_index].z);
			}
		}
		main.utils.ApplyChanges(sv.chunk_index, ref vertices);
	}


	public void Flat(SelectedVertices sv, Vector3 hit_point, float force) 
	{
		
		Vector3[] vertices = main.chunks[sv.chunk_index].mesh_filter.sharedMesh.vertices;

		foreach(SV s in sv.vertices_index) 
		{
			if (vertices [s.serial_index].y > hit_point.y) 
			{
				vertices [s.serial_index] -= new Vector3 (0, force, 0);
			} 
			else 
			{
				vertices [s.serial_index] += new Vector3 (0, force, 0);
			}
		}

		main.utils.ApplyChanges(sv.chunk_index, ref vertices);
	}



	public void FlatToAverage(SelectedVertices sv, float average, float force) 
	{

		Vector3[] vertices = main.chunks[sv.chunk_index].mesh_filter.sharedMesh.vertices;

		foreach(SV s in sv.vertices_index) 
		{
			if (vertices [s.serial_index].y > average) 
			{
				vertices [s.serial_index] -= new Vector3 (0, force, 0);
			} 
			else 
			{
				vertices [s.serial_index] += new Vector3 (0, force, 0);
			}
		}

		main.utils.ApplyChanges(sv.chunk_index, ref vertices);

	}


	public void Raise(SelectedVertices sv, float force) 
	{		
		Vector3[] vertices = main.chunks[sv.chunk_index].mesh_filter.sharedMesh.vertices;


		foreach(SV s in sv.vertices_index) 
		{
			vertices [s.serial_index] += new Vector3 (0, force, 0);
		}

		main.utils.ApplyChanges(sv.chunk_index, ref vertices);
	}

	public void FlatRaise(List<SelectedVertices> sv, float force, Extremes extremes, float deviation) 
	{		
		
		VerticesCopy[] copy = new VerticesCopy[sv.Count];
		for(int i = 0; i < sv.Count; i++)
		{
			copy[i] = new VerticesCopy(main.chunks[sv[i].chunk_index].mesh_filter.sharedMesh.vertices);
		}


		float extreme_y = main.chunks[sv[0].chunk_index].mesh_filter.sharedMesh.vertices[0].y;


		SelectedVertices[] extreme = new SelectedVertices[sv.Count];

		for(int i = 0; i < sv.Count; i++)
		{
			extreme[i] = new SelectedVertices(sv[i].chunk_index, new List<SV>());
		}

		if(extremes == Extremes.Minimum)
		{
			for(int s = 0; s < sv.Count; s++)
			{
				for(int i = 0; i < sv[s].vertices_index.Count; i++)
				{
					if(copy[s].vertices[sv[s].vertices_index[i].serial_index].y < extreme_y)
					{
						extreme_y = copy[s].vertices[sv[s].vertices_index[i].serial_index].y;
					}
				}
			}
			for(int s = 0; s < sv.Count; s++)
			{
				for(int i = 0; i < sv[s].vertices_index.Count; i++)
				{
					if(copy[s].vertices[sv[s].vertices_index[i].serial_index].y <= extreme_y + deviation)
					{
						extreme[s].vertices_index.Add(sv[s].vertices_index[i]);
					}
				}
			}
		}
		else
		{
			for(int s = 0; s < sv.Count; s++)
			{
				for(int i = 0; i < sv[s].vertices_index.Count; i++)
				{
					if(copy[s].vertices[sv[s].vertices_index[i].serial_index].y > extreme_y)
					{
						extreme_y = copy[s].vertices[sv[s].vertices_index[i].serial_index].y;
					}
				}
			}
			for(int s = 0; s < sv.Count; s++)
			{
				for(int i = 0; i < sv[s].vertices_index.Count; i++)
				{
					if(copy[s].vertices[sv[s].vertices_index[i].serial_index].y >= extreme_y - deviation)
					{
						extreme[s].vertices_index.Add(sv[s].vertices_index[i]);
					}
				}
			}
		}

		for(int i = 0; i < sv.Count; i++)
		{
			foreach (SV s in extreme[i].vertices_index) 
			{
				copy[i].vertices[s.serial_index] += new Vector3 (0, force, 0);
			}
			main.utils.ApplyChanges(extreme[i].chunk_index, ref copy[i].vertices);
		}
		
	}
	private class ChangesToApply
	{
		public int chunk_index;
		public List<int> indexes;
		Vector3[] vertices;
		public ChangesToApply(int lenght)
		{
			vertices = new Vector3[lenght];
		}
	}


	public void Randomize(SelectedVertices sv, Vector3 random)
	{
		Vector3[] vertices = main.chunks[sv.chunk_index].mesh_filter.sharedMesh.vertices;

		int lenght = main.terrain_creator.width * main.terrain_creator.lenght; 


		bool left_chunk = main.utils.LeftChunk(sv.chunk_index);
		bool right_chunk = main.utils.RightChunk(sv.chunk_index);
		bool upper_chunk = main.utils.UpperChunk(sv.chunk_index);
		bool lower_chunk = main.utils.LowerChunk(sv.chunk_index);



		foreach(SV s in sv.vertices_index)
		{
			//Vector3 random_vector = new Vector3(Random.Range(-random,random),0, Random.Range(-random,random));
			vertices[s.serial_index] += random;


			if(main.connected_vertices[s.serial_index] == null) continue;

			foreach(ConnectedVertices cv in main.connected_vertices[s.serial_index])
			{
				Vector3[] copy = main.chunks[sv.chunk_index + cv.chunk_index_shift].mesh_filter.sharedMesh.vertices;
				copy[cv.vert_index] = vertices[s.serial_index];
				main.utils.ApplyChanges(sv.chunk_index + cv.chunk_index_shift, ref copy);
			}
		}
		main.utils.ApplyChanges(sv.chunk_index, ref vertices);	
	}
}
