using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    public Slider ZoomSlider;
    public TextMeshProUGUI ZoomText;

    public Slider SmoothSlider;
    public TextMeshProUGUI SmoothText;

    public Slider MultSlider;
    public TextMeshProUGUI MultText;

    public Slider IsoSlider;
    public TextMeshProUGUI IsoText;

    public Toggle ClassicNoise;
    public Toggle PerlinNoise;
    public Toggle SimplexNoise;

    public Toggle Use3DNoise;

    public GameObject GenerateMenu;
    public GameObject GenerateScreen;
    public GameObject WinScreen;

    public GameObject MazeInfoScreen;
    public TextMeshProUGUI MazeInfoText;

    public delegate void MazeRestarted();
    public static event MazeRestarted OnMazeRestarted;

    private void Start()
    {
        GenerateMenu.SetActive(true);

        Maze.OnMazeGenerated += HideLoader;

        Player.OnMazeSolved += ShowWinScreen;
    }

    public void Play()
    {
        GenerateScreen.SetActive(true);

        NoiseParameters.Scale = ZoomSlider.value;
        NoiseParameters.Smooth = SmoothSlider.value;
        NoiseParameters.Mult = (int)MultSlider.value;
        NoiseParameters.IsoLevel = IsoSlider.value;

        NoiseParameters.ClassicNoise = ClassicNoise.isOn;
        NoiseParameters.PerlinNoise = PerlinNoise.isOn;
        NoiseParameters.SimplexNoise = SimplexNoise.isOn;

        NoiseParameters.Use3DNoise = Use3DNoise.isOn;


        GenerateMenu.SetActive(false);

        MazeInfoScreen.SetActive(true);
        MazeInfoText.text = CreateMazeConfigText();

    }

    public void RestartMaze()
    {
        OnMazeRestarted();

        WinScreen.SetActive(false);
        GenerateMenu.SetActive(true);
        MazeInfoScreen.SetActive(false);
    }

    public void Update()
    {
        // Updating UI texts for sliders value
        ZoomText.text = ZoomSlider.value.ToString("0.0");
        SmoothText.text = SmoothSlider.value.ToString("0.0");
        MultText.text = MultSlider.value.ToString();
        IsoText.text = IsoSlider.value.ToString("0.0");
    }

    private void HideLoader()
    {
        GenerateScreen.SetActive(false);
        MazeInfoScreen.SetActive(true);
    }

    private void ShowWinScreen()
    {
        WinScreen.SetActive(true);
        MazeInfoScreen.SetActive(false);
    }

    private string CreateMazeConfigText()
    {
        string mazeConfigText =
            $"Maze config:\n\n" +
            $"Scale: {ZoomSlider.value}\n" +
            $"Smooth: {SmoothSlider.value}\n" +
            $"Mult: {MultSlider.value}\n" +
            $"Iso level: {IsoSlider.value}\n\n" +
            $"Used noises: \n" +
            $"{(ClassicNoise.isOn ? "Classic\n" : "")}" +
            $"{(PerlinNoise.isOn ? "Perlin\n" : "")}" +
            $"{(SimplexNoise.isOn ? "Simplex\n" : "")}" +
            $"Use 3D noise: {(Use3DNoise.isOn ? "yes" : "no")}";

        return mazeConfigText;
    }

    public void CloseApp()
    {
        Application.Quit();
    }
}
