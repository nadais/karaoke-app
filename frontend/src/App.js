import React, { useState, useEffect } from 'react';
import { AgGridColumn, AgGridReact } from 'ag-grid-react';

import 'ag-grid-community/dist/styles/ag-grid.css';
import 'ag-grid-community/dist/styles/ag-theme-alpine.css';

function App() {
    const [rowData, setRowData] = useState([]);
    const [gridApi, setGridApi] = useState(null);
    const [loading, setLoading] = useState(false);
    const [catalog, setCatalog] = useState({});
    const [currentCatalog, setCurrentCatalog] = useState();
    const [gridColumnApi, setGridColumnApi] = useState(null);
    const searchDivStyle = { backgroundColor: "#dedede", padding: 10 }
    const searchStyle = {
        width: "100%", padding: "10px 20px", borderRadius: 20, outline: 0,
        border: "2px #68bf40 solid", fontSize: "100%"
    }

    const onGridReady = (params) => {
        setGridApi(params.api);
        setGridColumnApi(params.columnApi);
    };
    const onFilterTextChange = (e) => {
        gridApi.setQuickFilter(e.target.value)
    }
    useEffect(() => {
        fetchSongsRemotely();
    }, []);
    function getLoading() {
        if (loading) {
            return <div class="spinner-border" role="status">
                <span class="visually-hidden">Loading...</span>
            </div>
        }
    }
    function fetchSongsRemotely() {
        setLoading(true);
        fetch('https://karaoke-juliane.herokuapp.com/songs')
            .then(result => {
                var test = result.json();
                return test;
            })
            .then(response => {
                setLoading(false);
                setCatalog(response.songGroups);
                updateCatalog(Object.keys(catalog)[0]);
            });
    }

    function updateCatalog(catalogName)
    {
        setCurrentCatalog(catalogName);
        setRowData(catalog[catalogName]);
    }
    function onCatalogChanged(event)
    {
        updateCatalog(event.target.value);
    }

    return (
        <div>
            <h1 align="center">Karaoke night</h1>
            <label for="catalog">Select catalog</label>
            <select name='catalog' onChange={onCatalogChanged} value={currentCatalog}>
                {Object.keys(catalog).map(key =>
                    (<option value={key}>{key}</option>))}
            </select>
            <button onClick={fetchSongsRemotely}>
                Reload Songs
            </button>
            {getLoading()}
            <div style={searchDivStyle}>
                <input type="search" style={searchStyle} onChange={onFilterTextChange} placeholder="search songs..." />
            </div>

            <div className="ag-theme-alpine" style={{ height: 600 }}>
                <AgGridReact rowData={rowData}
                    defaultColDef={{
                        flex: 1,
                        sortable: true,
                        resizable: true,
                        filter: true,
                    }}
                    onGridReady={onGridReady}>
                    <AgGridColumn field="number" sortable={true} filter={true} />
                    <AgGridColumn field="name" sortable={true} filter={true} />
                    <AgGridColumn field="artist" sortable={true} filter={true} />
                </AgGridReact>
            </div>
        </div>
    );
};


export default App;