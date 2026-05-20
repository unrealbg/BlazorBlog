(function () {
    function normalizeQuillCodeBlocks(html) {
        if (!html || !html.includes("ql-code-block")) {
            return html || "";
        }

        const template = document.createElement("template");
        template.innerHTML = html;

        template.content.querySelectorAll(".ql-code-block-container").forEach((container) => {
            const lines = Array.from(container.querySelectorAll(".ql-code-block"))
                .map((line) => line.textContent || "");

            const pre = document.createElement("pre");
            const code = document.createElement("code");
            code.textContent = lines.join("\n");
            pre.appendChild(code);
            container.replaceWith(pre);
        });

        template.content.querySelectorAll(".ql-code-block").forEach((block) => {
            const pre = document.createElement("pre");
            const code = document.createElement("code");
            code.textContent = block.textContent || "";
            pre.appendChild(code);
            block.replaceWith(pre);
        });

        return template.innerHTML;
    }

    function getEditorHtml(quill) {
        const html = typeof quill.getSemanticHTML === "function"
            ? quill.getSemanticHTML()
            : quill.root.innerHTML;

        return normalizeQuillCodeBlocks(html);
    }

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
            return editor && editor.__quill ? getEditorHtml(editor.__quill) : "";
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
