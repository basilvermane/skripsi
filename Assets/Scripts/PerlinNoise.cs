using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Terrain))]
public class PerlinNoise : MonoBehaviour {

	public int TerrainWidth = 100;
	public int TerrainHeight = 100;
	public int TerrainGridSize = 100;

	[Range (0.0f, 1.0f)]
	public float whiteStart = 0.7f;
	[Range (0.0f, 1.0f)]
	public float whiteSpectrum = 0.2f;

	private Terrain terrain;
	private TerrainData terrainData;

	private Animator anim;

	private bool askGenerateNoise;
	private int tempWidth, tempHeight;
	private float tempMaxHeight;

	public Transform[] Walls;

	public int TLength {
		get {
			return terrain.terrainData.heightmapHeight;
		}
	}

	public int TWidth {
		get {
			return terrain.terrainData.heightmapWidth;
		}
	}

	public int THeight {
		get {
			return (int) terrain.terrainData.size.y;
		}
	}

	private void Awake () {
		askGenerateNoise = false;
	}

	// Use this for initialization
	void Start () {
		terrain = GetComponent<Terrain> ();
		terrainData = terrain.terrainData;
		TerrainWidth = terrainData.heightmapWidth;
		TerrainHeight = terrainData.heightmapHeight;
		anim = GetComponent<Animator> ();
		if (askGenerateNoise) {
			askGenerateNoise = false;
			GenerateNewNoise (tempWidth, tempHeight, tempMaxHeight, true);
		}
	}

	public void GenerateNewNoise (int width, int height, float maxHeight, bool createNewBall) {
		if (terrain == null) {
			askGenerateNoise = true;
			tempWidth = width;
			tempHeight = height;
			tempMaxHeight = maxHeight;
			return;
		}
		terrain.terrainData.size = new Vector3 (width, maxHeight, height);
		float[,] noise = Perlin2D (TerrainWidth, TerrainHeight, TerrainGridSize);
		terrainData.SetHeights (0, 0, noise);
		PaintTexture ();
		if (createNewBall)
			GameplayManager.Instance.SetBallGoal ();
		SetWalls (width, height);
	}

	public float PerlinInterpolate (float t) {
		return (6 * Mathf.Pow (t, 5)) - (15 * Mathf.Pow (t, 4)) + (10 * Mathf.Pow (t, 3));
	}

	public float GradientCalc (float x, float y, Vector2 grad) {
		return (x * grad.x) + (y * grad.y);
	}

	private void SetWalls (int width, int height) {
		float midX = width / 2.0f;
		float midZ = height / 2.0f;
		Walls[0].position = new Vector3 (midX, 0.0f, 0.0f);
		Walls[1].position = new Vector3 (0.0f, 0.0f, midZ);
		Walls[2].position = new Vector3 (midX, 0.0f, height);
		Walls[3].position = new Vector3 (width, 0.0f, midZ);

		float scaleWidth = width / 10.0f;
		float scaleHeight = height / 10.0f;

		Vector3 wallScale = Walls[0].localScale;

		wallScale.x = scaleWidth;
		Walls[0].localScale = Walls[2].localScale = wallScale;
		wallScale.x = scaleHeight;
		Walls[1].localScale = Walls[3].localScale = wallScale;
	}

	public float[,] Perlin2D (int width, int height, int gridsize) {
		float[,] result = new float[width, height];

		//generate random gradient at integer lattices (x, y)
		int d_width = (width / gridsize) + 2;
		int d_height = (height / gridsize) + 2;
		Vector2[,] grad = new Vector2[d_width, d_height];
		for (int i = 0; i < d_width; i++) {
			for (int j = 0; j < d_height; j++) {
				grad[i, j] = Random.insideUnitCircle;
				//normalize
				grad[i, j] = grad[i, j].normalized;
			}
		}

		for (int i = 0; i < width; i++) {
			float x = i * 1.0f / gridsize;
			int x_floor = Mathf.FloorToInt (x);
			float x_diff = x - x_floor;
			float x_interp = PerlinInterpolate (x_diff);

			for (int j = 0; j < height; j++) {
				float y = j * 1.0f / gridsize;
				int y_floor = Mathf.FloorToInt (y);
				float y_diff = y - y_floor;
				float y_interp = PerlinInterpolate (y_diff);

				//kalkulasi influence dari keempat integer lattice terdekat
				//atas, interpolasi kiri dan kanan
				float x1 = Mathf.Lerp (
					GradientCalc (x_diff, y_diff, grad[x_floor, y_floor]), //kiri atas
					GradientCalc (x_diff - 1, y_diff, grad[x_floor + 1, y_floor]), //kanan atas
					x_interp
				);
				//bawah, interpolasi kiri dan kanan
				float x2 = Mathf.Lerp (
					GradientCalc (x_diff, y_diff - 1, grad[x_floor, y_floor + 1]), //kiri bawah
					GradientCalc (x_diff - 1, y_diff - 1, grad[x_floor + 1, y_floor + 1]), //kanan bawah
					x_interp
				);
				//interpolasi atas dan bawah
				float val = Mathf.Lerp (x1, x2, y_interp);

				result[i, j] = ((val + 1.0f) / 2.0f) * 0.1f;
				//if (result[i, j] > 1.0f || result[i, j] < 0.0f) print (result[i, j]);
			}
		}

		return result;
	}

	private void PaintTexture () {
		float[,,] splatmapData = new float[
			terrainData.alphamapWidth,
			terrainData.alphamapHeight,
			terrainData.alphamapLayers];
		//print (terrainData.alphamapWidth + " " + terrainData.alphamapHeight);
		float minHeight = 0.0f, maxheight = 0.0f;
		for (int y = 0; y < terrainData.alphamapHeight; y++) {
			for (int x = 0; x < terrainData.alphamapWidth; x++) {
				float y_01 = (float) y / (float) terrainData.alphamapHeight;
				float x_01 = (float) x / (float) terrainData.alphamapWidth;
				float height = terrainData.GetHeight (
					Mathf.RoundToInt (y_01 * terrainData.heightmapHeight),
					Mathf.RoundToInt (x_01 * terrainData.heightmapWidth));
				//print (height);
				if (y == 0 && x == 0) {
					minHeight = maxheight = height;
				} else {
					if (height < minHeight) minHeight = height;
					if (height > maxheight) maxheight = height;
				}
			}
		}
		float whiteStartVal = minHeight + (whiteStart * (maxheight - minHeight));
		float whiteSpectrumVal = whiteSpectrum * (maxheight - minHeight);
		for (int y = 0; y < terrainData.alphamapHeight; y++) {
			for (int x = 0; x < terrainData.alphamapWidth; x++) {
				float y_01 = (float) y / (float) terrainData.alphamapHeight;
				float x_01 = (float) x / (float) terrainData.alphamapWidth;
				float height = terrainData.GetHeight (
					Mathf.RoundToInt (y_01 * terrainData.heightmapHeight),
					Mathf.RoundToInt (x_01 * terrainData.heightmapWidth));
				float[] splatWeights = new float[terrainData.alphamapLayers];
				if (height < whiteStartVal - whiteSpectrumVal) {
					//hijau total
					splatWeights[0] = 1.0f;
					splatWeights[1] = 0.0f;
				} else if (height > whiteStartVal + whiteSpectrumVal) {
					//putih total
					splatWeights[0] = 0.0f;
					splatWeights[1] = 1.0f;
				} else {
					//campuran
					splatWeights[1] = (height - (whiteStartVal - whiteSpectrumVal)) / (2 * whiteSpectrumVal);
					splatWeights[0] = 1.0f - splatWeights[1];
				}

				for (int i = 0; i < terrainData.alphamapLayers; i++) {
					splatmapData[x, y, i] = splatWeights[i];
				}
			}
		}
		//print (minHeight + " " + maxheight);

		terrainData.SetAlphamaps (0, 0, splatmapData);
	}

	/*private void Update () {
		if (Input.GetKeyDown (KeyCode.U)) {
			int w, h;
			float mh;
			w = Random.Range (100, 200);
			h = Random.Range (100, 200);
			mh = Random.Range (300.0f, 400.0f);
			print (w + " " + h + " " + mh);
			GenerateNewNoise (w, h, mh);
		}
	}*/

	public float GetCurrentHeightAt (float x, float y) {
		float y_01 = (float) y / (float) terrainData.alphamapHeight;
		float x_01 = (float) x / (float) terrainData.alphamapWidth;
		float height = terrainData.GetHeight (
			Mathf.RoundToInt (y_01 * terrainData.heightmapHeight),
			Mathf.RoundToInt (x_01 * terrainData.heightmapWidth));
		return height;
	}

	public void ShowTerrain () {
		anim.SetBool ("show", true);
	}

	public void HideTerrain () {
		anim.SetBool ("show", false);
	}
}