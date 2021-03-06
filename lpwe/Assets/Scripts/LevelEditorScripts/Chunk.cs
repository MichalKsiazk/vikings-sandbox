﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk {

	public GameObject plane;

	public GameObject visual;

	public MeshFilter mesh_filter;

	public int chunk_index_x;
	public int chunk_index_z;

	public int chunk_serial_index; 

	public Color[] colors;

	public Chunk(GameObject chunk_prefab, Vector3 position, int chunk_index_x, int chunk_index_z, int chunk_serial_index) {

		this.chunk_index_x = chunk_index_x;
		this.chunk_index_z = chunk_index_z;

		this.chunk_serial_index = chunk_serial_index;

		plane = GameObject.Instantiate(chunk_prefab);
		plane.transform.position = position;

		visual = RenderMesh(plane);

		mesh_filter = plane.GetComponent<MeshFilter> ();
		Mesh mesh_copy = Mesh.Instantiate(mesh_filter.sharedMesh) as Mesh;
		mesh_filter.mesh = mesh_copy;

	}


	public Chunk(GameObject game_object, int serial_index, int width) {

		this.chunk_serial_index = serial_index;
		this.chunk_index_x = serial_index % width;
		this.chunk_index_z = chunk_index_z / width;

		plane = game_object;

		visual = RenderMesh(plane);

		mesh_filter = plane.GetComponent<MeshFilter> ();
		Mesh mesh_copy = Mesh.Instantiate(mesh_filter.sharedMesh) as Mesh;
		mesh_filter.mesh = mesh_copy;

	}

	GameObject RenderMesh(GameObject plane){
		Transform[] childs = plane.GetComponentsInChildren<Transform> ();
		foreach(Transform t in childs)
		{
			if(t.gameObject.name == "visual")
			{
				return t.gameObject;
			}
		}
		return null;
	} 

}