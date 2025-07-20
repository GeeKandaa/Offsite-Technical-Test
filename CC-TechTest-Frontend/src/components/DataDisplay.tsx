import './DataDisplay.css';
import { useState, useRef } from "react";
import { getAllEntries, getEntryByMpan, getEntryBySerial, getEntryByDate, getEntryByAddress, getEntryByPostcode } from "../external/backend";

//#region Interfaces
interface RowDefinition {
    mpan: BigInt;
    meterSerial : string;
    dateOfInstallation: Date;
    addressLine1: string;
    postcode : string;
}
//#endregion
//region Constants
const SEARCH_FIELDS = [
    { value: 'viewAll', label: 'Search by..' },
    { value: 'mpan', label: 'MPAN' },
    { value: 'meterSerial', label: 'Meter Serial' },
    { value: 'dateOfInstallation', label: 'Date of Installation' },
    { value: 'addressLine1', label: 'Address' },
    { value: 'postcode', label: 'Postcode' }
]
//endregion

const DataDisplay = ({ DarkMode }: { DarkMode: Boolean }) => {
    //#region Component Variables
    // State variables
    const [entries, setEntries] = useState<Array<RowDefinition>>([]);
    const [singleEntry, setSingleEntry] = useState<RowDefinition | null>(null);
    const [error, setError] = useState<string>('');
    const [isModalOpen, setIsModalOpen] = useState<boolean>(false);

    // Search
    const [searchType, setSearchType] = useState<string>('viewAll');
    const [searchValue, setSearchValue] = useState<string>('');
    const debounceRef = useRef<number | null>(null);
    //#endregion

    //#region Search Event Handlers
    /**
     * Handles the searchbar input change, performing the search after the specified delay.
     * @param value - The search input value.
     */
    const handleSearchChange = (value: string) => {
        setSearchValue(value);

        // restart the timer if it has not fired to allow time for user to type query string
        if (debounceRef.current) clearTimeout(debounceRef.current);

        debounceRef.current = setTimeout(() => {
            performSearch(searchType, value);
        }, 500); //ms
    }

    /**
     * Handles the change of search type, resetting the search value and entries.
     * @param e - The change event from the select input.
     */
    const handleTypeChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
        setSearchType(e.target.value);
        setSearchValue('');
        setEntries([]);
        setSingleEntry(null);
    };

    /**
     * Sends an appropriate GET request as per search type and value
     * @remarks
     * This is automatically called as part of the {@link handleSearchChange} event handler
     * @param type - The type of search to be performed
     * @param value - The search input value
     */
    const performSearch = async (type: string, value: string) => {
        // Clear already existing results
        setError('');
        setEntries([]);
        setSingleEntry(null);

        // prevent empty search requests
        if (!value) return;

        try {
            let data;
            switch (type) {
                case 'mpan':
                    data = await getEntryByMpan(value);
                    break;
                case 'meterSerial':
                    data = await getEntryBySerial(value);
                    break;
                case 'dateOfInstallation':
                    const [year, month, day] = value.split('-');
                    data = await getEntryByDate(`${year}${month}${day}`);
                    break;
                case 'addressLine1':
                    data = await getEntryByAddress(value);
                    break;
                case 'postcode':
                    data = await getEntryByPostcode(value);
                    break;
                default:
                    throw new Error('Invalid search type');
            }

            setEntries(data);
        } catch (err) {
            setError((err as Error).message);
        }
    }
    //#endregion

    //#region Button Event Handlers
    /**
     * Sends a GET request to retrieve all entries and handles the response
     */
    const FetchAllEntries = async () => {
        setError('');
        try {
            const data = await getAllEntries();
            setEntries(data);
            setSingleEntry(null);
        } catch (err) {
            setError((err as Error).message);
        }
    }

    /**
     * Parses loaded entry data and initiates download to users local machine
     */
    const DownloadData = () => {
        // Prevent downloading files with no data..
        if (!entries.length) return;

        // Convert data to string values that conform to upload requirements.
        const header = "MPAN|MeterSerial|DateOfInstallation|AddressLine1|PostCode";
        const rows = entries.map(row =>
            [
                row.mpan.toString(),
                row.meterSerial,
                row.dateOfInstallation
                    ? new Date(row.dateOfInstallation).toISOString().slice(0, 10).replace(/-/g, "")
                    : "",
                row.addressLine1 ? row.addressLine1.replace(/\|/g, ";") : "",
                row.postcode ? row.postcode.replace(/\|/g, ";") : ""
            ].join("|")
        );
        const content = [header, ...rows].join("\r\n");

        // Create a temporary blob and URL for the formatted entry data
        const blob = new Blob([content], { type: "text/plain" });
        const url = URL.createObjectURL(blob);

        // Create a temporary anchor element to trigger the download
        const a = document.createElement("a");
        a.href = url;
        a.download = "CC-TechTest-App-Download.txt";
        // Add anchor to dom, "click" it and then remove it and release the blob URL
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
        URL.revokeObjectURL(url);
    };
    //#endregion

    //#region Render Functions
    /**
     * @param rows - Loaded data entries
     * @returns Rendered table containing entry data
     */
    const RenderTable = (rows: RowDefinition[]) => (
        <table border={1} cellPadding={6} style={{ marginTop: '10px', width: '100%' }}>
            <thead>
                <tr>
                    <th>MPAN</th>
                    <th>Meter Serial</th>
                    <th>Date of Installation</th>
                    <th>Address Line 1</th>
                    <th>Post Code</th>
                </tr>
            </thead>
            <tbody>
                {rows.map((row, index) => (
                    <tr key={index}>
                        <td>{row.mpan.toString()}</td>
                        <td>{row.meterSerial}</td>
                        <td>{new Date(row.dateOfInstallation).toLocaleDateString()}</td>
                        <td>{row.addressLine1}</td>
                        <td>{row.postcode}</td>
                    </tr>
                ))}
            </tbody>
        </table>
    );
    //#endregion

    //#region Render
    return (
        <div className="container">
            <div className="controls">
                {/*Search type selection*/}
                <select
                    className="select"
                    style={{
                        '--border': DarkMode ? '#666' : '#ccc',
                        '--bg': DarkMode ? '#333' : '#fff',
                        '--color': DarkMode ? '#fff' : '#000'
                    } as React.CSSProperties}
                    value={searchType}
                    onChange={handleTypeChange}
                >
                    {/*viewAll is controlled and labelled by accompanying button,
                     We use a dynamic label here so that a user is aware they can change to search functions when this is selected*/}
                    <option value="viewAll">
                        {searchType === 'viewAll' ? 'Search by..' : 'View All Entries'}
                    </option>
                    {SEARCH_FIELDS.slice(1).map(f => (
                        <option key={f.value} value={f.value}>{f.label}</option>
                    ))}
                </select>
                {/* Search controls*/}
                {searchType === 'viewAll' ? (
                    <button onClick={FetchAllEntries}>View All Entries</button>) :
                    searchType === "dateOfInstallation" ? (
                        <input
                            className="input"
                            style={{
                                '--border': DarkMode ? '#666' : '#ccc',
                                '--bg': DarkMode ? '#333' : '#fff',
                                '--color': DarkMode ? '#fff' : '#000'
                            } as React.CSSProperties}
                            type="date"
                            value={searchValue}
                            onChange={e => handleSearchChange(e.target.value)}
                        />
                    ) : (
                            <input
                                className="input"
                                style={{
                                    '--border': DarkMode ? '#666' : '#ccc',
                                    '--bg': DarkMode ? '#333' : '#fff',
                                    '--color': DarkMode ? '#fff' : '#000'
                                } as React.CSSProperties}
                                type="text"
                                placeholder={`Search by ${SEARCH_FIELDS.find(f => f.value === searchType)?.label}`}
                                value={searchValue}
                                onChange={e => handleSearchChange(e.target.value)}
                            />
                    )}
            </div>
            {/*Error display*/}
            <div className="error">
                {error && <p style={{ color: 'red' }}>Error: {error}</p>}
            </div>
            {/*In-line results table*/}
            <div className="table-container"
                style={{
                    '--table-border': DarkMode ? '#444' : '#ccc',
                    '--table-bg': DarkMode ? '#1e1e1e' : '#f9f9f9'
                } as React.CSSProperties}>
                {entries.length > 0 && (
                    
                    <div className = "sticky-btns">
                        <button
                            className="action-btn"
                            style={{
                                '--btn-bg': DarkMode ? '#333' : '#e0e0e0',
                                '--btn-color': DarkMode ? '#fff' : '#000',
                                '--btn-border': DarkMode ? '#666' : '#ccc'
                            } as React.CSSProperties}
                            onClick={() => DownloadData()}
                            title="Download data">
                            📥
                        </button>
                        <button
                            className="action-btn"
                            style={{
                                '--btn-bg': DarkMode ? '#333' : '#e0e0e0',
                                '--btn-color': DarkMode ? '#fff' : '#000',
                                '--btn-border': DarkMode ? '#666' : '#ccc'
                            } as React.CSSProperties}
                            onClick={() => setIsModalOpen(true)}
                            title="Expand view">
                            🔍
                        </button>
                    </div>
                )}
                {entries.length > 0 && (
                    <div className="table-margin">
                        {RenderTable(entries)}
                    </div>
                )}
                {entries.length === 0 && !singleEntry && (
                    <p className="empty">No data loaded yet.</p>
                )}
            </div>
            {isModalOpen && (
                <div
                    className="modal-overlay"
                    onClick={() => setIsModalOpen(false)} >
                    <div className="modal-btns" >
                        <button
                            className="modal-btn"
                            style={{
                                '--modal-btn-bg': DarkMode ? '#444' : '#fff',
                                '--modal-btn-color': DarkMode ? '#fff' : '#000',
                                '--modal-btn-border': DarkMode ? '#666' : '#ccc'
                            } as React.CSSProperties}
                            onClick={(e) => {
                            e.stopPropagation();
                            setIsModalOpen(false);
                            }}
                            title="Close expanded view" >
                            ❌ Close
                        </button>
                        <button
                            className="modal-btn"
                            style={{
                                '--modal-btn-bg': DarkMode ? '#444' : '#fff',
                                '--modal-btn-color': DarkMode ? '#fff' : '#000',
                                '--modal-btn-border': DarkMode ? '#666' : '#ccc',
                                width: '144px'
                            } as React.CSSProperties}
                            onClick={(e) => {
                                e.stopPropagation();
                                DownloadData();
                            }}
                            title="Download data" >
                            📥 Download
                        </button>
                    </div>
                    <div
                        className="modal-content"
                        style={{
                            '--modal-bg': DarkMode ? '#1e1e1e' : '#fff',
                            '--modal-color': DarkMode ? '#e0e0e0' : '#000'
                        } as React.CSSProperties}
                        onClick={e => e.stopPropagation()} >
                        <hr />
                        <div className="table-margin">
                            {RenderTable(entries)}
                        </div>
                    </div>
                </div>
            )}
        </div>
    );
};
//#endregion

export default DataDisplay;