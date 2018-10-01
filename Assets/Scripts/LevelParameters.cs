public class LevelParam {
	private float gravity;
	private int heightMapWidth;
	private int heightMapHeight;
	private float maxHeight;
	private float minDistance;

	public LevelParam (
		float gravity,
		int width, 
		int height, 
		float maxHeight,
		float minDistance) {
		this.gravity = gravity;
		this.heightMapHeight = height;
		this.heightMapWidth = width;
		this.maxHeight = maxHeight;
		this.minDistance = minDistance;
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
}

public static class LevelParameters {
	public static int SandboxDefaultLevel {
		get {
			return LevelParams.Length - 1;
		}
	}

	public static LevelParam[] LevelParams = {
		new LevelParam(9.8f, 50, 50, 100, 20.0f), //1
		new LevelParam(9.8f, 100, 100, 300, 50.0f), //2
		new LevelParam(9.8f, 300, 300, 300, 100.0f), //3
		new LevelParam(9.8f, 100, 100, 600, 50.0f), //4
		new LevelParam(9.8f, 500, 500, 100, 300.0f), //5
		new LevelParam(20.0f, 100, 100, 100, 50.0f), //6
		new LevelParam(9.8f, 500, 500, 600, 200.0f), //7
		new LevelParam(5.0f, 200, 200, 200, 100.0f), //8
		new LevelParam(5.0f, 500, 500, 600, 200.0f), //9
		new LevelParam(20.0f, 500, 500, 600, 300.0f), //10

		new LevelParam(9.8f, 300, 300, 600, 100.0f), //sandbox default
	};
}
