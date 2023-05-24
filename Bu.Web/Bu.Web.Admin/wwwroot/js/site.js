function applyMonacoEditor(textAreaId, selectLangaugeId, monacoEditorBasePath) {
    const textArea = document.getElementById(textAreaId);
    const divTextArea = document.createElement("div");
    const selectLangauge = selectLangaugeId != null ? document.getElementById(selectLangaugeId) : null;
    divTextArea.id = textAreaId + "Div";
    textArea.parentElement.appendChild(divTextArea);
    divTextArea.insertAdjacentElement("afterend", textArea);
    divTextArea.style.minHeight = "500px";
    var readOnly = textArea.readOnly == true || textArea.parentElement.closest("[disabled]") != null;
    const language = selectLangauge != null ? selectLangauge.options[selectLangauge.selectedIndex].text.toLowerCase() : defaultLanguage;
    if (language == null) {
        throw "Provide defaultLanguage";
    }

    require.config({ paths: { 'vs': monacoEditorBasePath + "min/vs" } });
    require(['vs/editor/editor.main'], function () {
        let theme = "vs";
        if (window.matchMedia("(prefers-color-scheme: dark)").matches) {
            theme = "vs-dark";
        }

        window.editor = monaco.editor.create(divTextArea, {
            value: textArea.value,
            language: language,
            readOnly: readOnly,
            lineNumbers: "on",
            wordWrap: "bounded",
            wordWrapColumn: 100,
            wrappingIndent: "same",
            fontSize: 14,
            roundedSelection: false,
            scrollBeyondLastLine: false,
            quickSuggestions: false,
            minimap: { enabled: true },
            theme: theme,
            automaticLayout: true,
            scrollbar: {
                alwaysConsumeMouseWheel: false,
            }
        });
        textArea.hidden = true;
    });

    if (!readOnly && selectLangauge != null) {
        if (textArea.form != null) {
            textArea.form.onsubmit = function () {
                textArea.value = window.editor.getValue();
            };
        }

        selectLangauge.onchange=function () {
            window.editor.language = selectLangauge.options[selectLangauge.selectedIndex].text.toLowerCase();
        };
    }
}