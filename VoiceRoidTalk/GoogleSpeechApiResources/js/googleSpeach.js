var recondingResult = document.getElementById("RecondingResult");
var statusText = document.getElementById("Status");
var isSpeech = false;

function voiceRecognition() {
    SpeechRecognition = webkitSpeechRecognition || SpeechRecognition;
    const recognition = new SpeechRecognition();
    recognition.lang = "ja-JP";
    recognition.interimResults = true;
    recognition.continuous = true;

    recognition.onsoundstart = () => {
        statusText.innerHTML = '認識中...';
    }

    recognition.onnomatch = () => {
        statusText.innerHTML = 'もう一度試してみてください';
        if (isSpeech === false) {
            voiceRecognition();
        }
    }

    recognition.onerror = () => {
        statusText.innerHTML = 'エラー';
        if (isSpeech === false) {
            voiceRecognition();
        }
    }

    recognition.onsoundend = () => {
        statusText.innerHTML = '停止中';
        if (isSpeech === false) {
            voiceRecognition();
        }
    }

    recognition.onresult = (event) => {

        let results = event.results;

        for (let i = event.resultIndex; i < results.length; i++) {
            if (results[i].isFinal) {
                recondingResult.innerText = results[i][0].transcript;
                voiceRecognition();
            }
            else {
                isSpeech = true;
            }
        }
    }

    isSpeech = false;
    statusText.innerHTML = '話しかけてください';

    recognition.start();
}

voiceRecognition();