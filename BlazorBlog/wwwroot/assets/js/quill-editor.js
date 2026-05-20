(function () {
    window.blazorBlogQuill = {
        initialize: function (editorId, toolbarId, html, placeholder) {
            const editor = document.getElementById(editorId);
            if (!editor || editor.__quill) {
                return;
            }

            const quill = new Quill(editor, {
                modules: {
                    toolbar: "#" + toolbarId
                },
                placeholder: placeholder,
                theme: "snow"
            });

            if (html) {
                quill.clipboard.dangerouslyPasteHTML(html);
            }

            editor.__quill = quill;
        },

        getHtml: function (editorId) {
            const editor = document.getElementById(editorId);
            return editor && editor.__quill ? editor.__quill.root.innerHTML : "";
        },

        getText: function (editorId) {
            const editor = document.getElementById(editorId);
            return editor && editor.__quill ? editor.__quill.getText() : "";
        },

        setHtml: function (editorId, html) {
            const editor = document.getElementById(editorId);
            if (!editor || !editor.__quill) {
                return;
            }

            editor.__quill.setContents([]);
            if (html) {
                editor.__quill.clipboard.dangerouslyPasteHTML(html);
            }
        },

        dispose: function (editorId) {
            const editor = document.getElementById(editorId);
            if (editor) {
                delete editor.__quill;
            }
        }
    };
})();
