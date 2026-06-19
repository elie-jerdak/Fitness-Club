const {
    InlineEditor,
    AutoImage,
    Autoformat,
    Autosave,
    ImageBlock,
    BlockQuote,
    Bold,
    CloudServices,
    Essentials,
    Heading,
    ImageCaption,
    ImageInsertViaUrl,
    ImageStyle,
    ImageTextAlternative,
    ImageToolbar,
    ImageUpload,
    ImageInline,
    Indent,
    IndentBlock,
    Italic,
    Link,
    LinkImage,
    List,
    Paragraph,
    Table,
    TableToolbar,
    TextTransformation,
    TodoList,
    Underline
} = window.CKEDITOR;

const LICENSE_KEY =
    'eyJhbGciOiJFUzI1NiJ9.eyJleHAiOjE3NzgxOTgzOTksImp0aSI6ImU1OTdlMTZkLTlkMDQtNDQyMS1hMjA5LTQyOGY3NGEzZTA3MSIsInVzYWdlRW5kcG9pbnQiOiJodHRwczovL3Byb3h5LWV2ZW50LmNrZWRpdG9yLmNvbSIsImRpc3RyaWJ1dGlvbkNoYW5uZWwiOlsiY2xvdWQiLCJkcnVwYWwiLCJzaCJdLCJ3aGl0ZUxhYmVsIjp0cnVlLCJsaWNlbnNlVHlwZSI6InRyaWFsIiwiZmVhdHVyZXMiOlsiKiJdLCJ2YyI6ImViYmQxMmY5In0.2jE1BtL-1ikoOfhCgdsch0HPc72T1O6EtxPOrEnBoHwvOPxlMopHt_MnngnH_0LTz5sUjDEOAN42eMXW0KW9rg';

function buildConfig(initialData) {
    return {
        plugins: [
            Autoformat, AutoImage, Autosave, BlockQuote, Bold,
            CloudServices, Essentials, Heading, ImageBlock, ImageCaption,
            ImageInline, ImageInsertViaUrl, ImageStyle, ImageTextAlternative,
            ImageToolbar, ImageUpload, Indent, IndentBlock, Italic,
            Link, LinkImage, List, Paragraph, Table, TableToolbar,
            TextTransformation, TodoList, Underline
        ],
        licenseKey: LICENSE_KEY,
        placeholder: 'Type or paste your content here!',
        initialData: initialData || '',
        toolbar: {
            items: [
                'undo', 'redo', '|',
                'heading', '|',
                'bold', 'italic', 'underline', '|',
                'link', 'insertTable', 'blockQuote', '|',
                'bulletedList', 'numberedList', 'todoList',
                'outdent', 'indent'
            ],
            shouldNotGroupWhenFull: false
        },
        heading: {
            options: [
                { model: 'paragraph', title: 'Paragraph', class: 'ck-heading_paragraph' },
                { model: 'heading1', view: 'h1', title: 'Heading 1', class: 'ck-heading_heading1' },
                { model: 'heading2', view: 'h2', title: 'Heading 2', class: 'ck-heading_heading2' },
                { model: 'heading3', view: 'h3', title: 'Heading 3', class: 'ck-heading_heading3' },
                { model: 'heading4', view: 'h4', title: 'Heading 4', class: 'ck-heading_heading4' },
                { model: 'heading5', view: 'h5', title: 'Heading 5', class: 'ck-heading_heading5' },
                { model: 'heading6', view: 'h6', title: 'Heading 6', class: 'ck-heading_heading6' }
            ]
        },
        image: {
            toolbar: [
                'toggleImageCaption', 'imageTextAlternative', '|',
                'imageStyle:inline', 'imageStyle:wrapText', 'imageStyle:breakText'
            ]
        },
        link: {
            addTargetToExternalLinks: true,
            defaultProtocol: 'https://',
            decorators: {
                toggleDownloadable: {
                    mode: 'manual',
                    label: 'Downloadable',
                    attributes: { download: 'file' }
                }
            }
        },
        table: {
            contentToolbar: ['tableColumn', 'tableRow', 'mergeTableCells']
        }
    };
}