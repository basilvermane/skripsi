public class LevelParam {
	private float gravity;
	private int heightMapWidth;
	private int heightMapHeight;
	private float maxHeight;
	private float minDistance;
	private float ballMass;

	public LevelParam (
		float gravity,
		int width, 
		int height, 
		float maxHeight,
		float minDistance,
		float ballMass) {
		this.gravity = gravity;
		this.heightMapHeight = height;
		this.heightMapWidth = width;
		this.maxHeight = maxHeight;
		this.minDistance = minDistance;
		this.ballMass = ballMass;
	}

	public float Gravity {
		get { return gravity; }
	}

	public int HeightMapWidth {
		get { return heightMapHeight; }
	}

	public int HeightMapHeight {
		get { return heightMapHeight; }
	}

	public float MaxHeight {
		get { return maxHeight; }
	}

	public float MinDistance {
		get { return minDistance; }
	}

	public float BallMass {
		get { return ballMass; }
	}
}

public static class LevelParameters {
	public static int SandboxDefaultLevel {
		get {
			return LevelParams.Length - 1;
		}
	}

	public static LevelParam[] LevelParams = {
		//1 = map kecil
		new LevelParam(9.8f, 50, 50, 100, 20.0f, 1f), //1

		//2 = map medium
		new LevelParam(9.8f, 100, 100, 300, 50.0f, 1f), //2

		//3 = map besar
		new LevelParam(9.8f, 300, 300, 300, 100.0f, 1f), //3

		//4 = map medium, tinggi
		new LevelParam(9.8f, 100, 100, 600, 50.0f, 1f), //4

		//5 = map terbesar, landai
		new LevelParam(9.8f, 500, 500, 100, 300.0f, 1f), //5

		//6 = map kecil, gravity besar
		new LevelParam(15.0f, 100, 100, 100, 50.0f, 1f), //6

		//7 = map medium, bola berat
		new LevelParam(9.8f, 300, 300, 300, 100.0f, 3f), //7

		//8 = map kecil, gravity kecil
		new LevelParam(5.0f, 200, 200, 200, 100.0f, 1f), //8

		//9 = map terbesar, gravity kecil
		new LevelParam(5.0f, 500, 500, 600, 200.0f, 1f), //9

		//10 = map terbesar, gravity besar, bola berat
		new LevelParam(15.0f, 500, 500, 600, 300.0f, 3f), //10

		new LevelParam(9.8f, 300, 300, 600, 100.0f, 1f), //sandbox default
	};
}
