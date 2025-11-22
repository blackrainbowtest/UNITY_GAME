using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

[System.Serializable]
public class HintData
{
    public string[] hints;
}

public class LoadingScreen : MonoBehaviour
{
    [Header("Cubes Animation")]
    public Image[] cubes;
    public float cubeDelay = 0.2f;

    [Header("Hints")]
    public Text hintText;
    public string language = "ru"; // язык для файла hints_ru.json
    public float hintDelay = 3f;

    private string[] hints;

    void Start()
    {
        LoadHints();
        StartCoroutine(AnimateCubes());
        StartCoroutine(ChangeHints());
        StartCoroutine(LoadNextScene());
    }

    void LoadHints()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("hints_" + language);
        if (jsonFile != null)
        {
            HintData data = JsonUtility.FromJson<HintData>(jsonFile.text);
            hints = data.hints;
        }
        else
        {
            Debug.LogWarning("Не найден JSON-файл для языка: " + language);
            hints = new string[] { "Подсказки не найдены." };
        }
    }

    IEnumerator AnimateCubes()
    {
        while (true)
        {
            for (int i = 0; i < cubes.Length; i++)
            {
                cubes[i].enabled = true;
                yield return new WaitForSeconds(cubeDelay);
            }
            for (int i = 0; i < cubes.Length; i++)
            {
                cubes[i].enabled = false;
                yield return new WaitForSeconds(cubeDelay);
            }
        }
    }

    IEnumerator ChangeHints()
    {
        int index = Random.Range(0, hints.Length);
        while (hints != null && hints.Length > 0)
        {
            hintText.text = "Подсказка:\n" + hints[index];
            index = (index + 1) % hints.Length;
            yield return new WaitForSeconds(hintDelay);
        }
    }

    // ↓↓↓ добавляем сюда в самый конец
    IEnumerator LoadNextScene()
    {
        // Подождём немного, чтобы анимация начала играть
        yield return new WaitForSeconds(1f);

        AsyncOperation operation = SceneManager.LoadSceneAsync(SceneLoader.sceneToLoad);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            // Когда загрузка почти закончена — активируем сцену
            if (operation.progress >= 0.9f)
            {
                yield return new WaitForSeconds(1f);
                operation.allowSceneActivation = true;
            }
            yield return null;
        }
    }
}
