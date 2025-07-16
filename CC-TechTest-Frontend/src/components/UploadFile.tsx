import './UploadFile.css';
import { useState, useRef } from 'react';
import { postFile, validateFile } from '../external/backend';

interface InvalidRowDefinition {
    mpan: BigInt;
    meterSerial: string;
    dateOfInstallation: Date;
    addressLine1: string;
    postcode: string;
}
const InvalidFieldFlags = {
    MPAN: 1,
    MeterSerial: 2,
    DateOfInstallation: 4,
    AddressLine1: 8,
    Postcode: 16
};

const UploadFile = ({ DarkMode } : {DarkMode:Boolean}) => {
    const [file, setFile] = useState<File | null>(null);
    const [successCount, setSuccessCount] = useState<number | null>(null);
    const [failureCount, setFailureCount] = useState<number | null>(null);
    const [failedRows, setFailedRows] = useState<InvalidRowDefinition[]>([]);
    const [message, setMessage] = useState<string>('');
    const fileInputRef = useRef<HTMLInputElement | null>(null);
    const [isModalOpen, setIsModalOpen] = useState<boolean>(false);

    const ClearFileDialog = async () => {
        if (fileInputRef.current) {
            fileInputRef.current.value = "";
        }
    }
    const ResetDisplay = async () => {
        setFile(null);
        ClearFileDialog();
        setSuccessCount(null);
        setFailureCount(null);
        setFailedRows([]);
        setMessage('');
    }
    const handleUpload = async () => {
        ResetDisplay();
        if (!file) {
            setMessage('Please select a file to upload.');
            setSuccessCount(null);
            setFailureCount(null);
            return;
        }
        if (!file.name.endsWith('.txt')) {
            setMessage('Only .txt files are allowed.');
            return;
        }

        try {
            const data = await postFile(file);
            setMessage(data.message);
            setSuccessCount(data.successCount);
            setFailureCount(data.failureCount);
        }
        catch (err) {
            setMessage((err as Error).message);
        }

        ClearFileDialog();
    };

    const handleValidation = async () => {
        ResetDisplay();
        if (!file) {
            setMessage('Please select a file to validate.');
            return;
        }
        if (!file.name.endsWith('.txt')) {
            setMessage('Only .txt files are allowed.');
            return;
        }

        try {
            const data = await validateFile(file);
            setMessage(data.message);
            setSuccessCount(data.successCount);
            setFailureCount(data.failureCount);
            setFailedRows(data.failedRows || []);
        }
        catch (err) {
            setMessage((err as Error).message);
        }

        if (fileInputRef.current) {
            fileInputRef.current.value = "";
        }
    };

    const RenderFailedTable = (rows: any[]) => (
        <table className="fullwidth-table" border={1} cellPadding={6}>
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
                        <td className={(row.failedField & InvalidFieldFlags.MPAN) ? "text-red" : undefined}>{row.mpan?.toString?.() ?? ''}</td>
                        <td className={(row.failedField & InvalidFieldFlags.MeterSerial) ? "text-red" : undefined}>{row.meterSerial ?? ''}</td>
                        <td className={(row.failedField & InvalidFieldFlags.DateOfInstallation) ? "text-red" : undefined}>{row.dateOfInstallation ? row.dateOfInstallation : ''}</td>
                        <td className={(row.failedField & InvalidFieldFlags.AddressLine1) ? "text-red" : undefined}>{row.addressLine1 ?? ''}</td>
                        <td className={(row.failedField & InvalidFieldFlags.Postcode) ? "text-red" : undefined}>{row.postcode ?? ''}</td>
                    </tr>
                ))}
            </tbody>
        </table>
    );

    return (
        <div>
            <input
                ref={fileInputRef}
                type="file"
                accept=".txt"
                onChange={(e) => setFile(e.target.files?.[0] || null)}
            />
            <button
                onClick={handleValidation}>Validate File
            </button>
            <button
                onClick={handleUpload}>Upload File
            </button>
            <p>{message}</p>
            {successCount !== null && <p>{successCount} rows successfully processed.</p>}
            {failureCount !== null && <p className="text-red">WARNING: {failureCount} rows failed to process.</p>}
            {failureCount !== null && failureCount > 0 && failedRows.length > 0 && (
                <div className="table-container"
                    style={{
                        '--border-color': DarkMode ? '#444' : '#ccc',
                        '--table-bg': DarkMode ? '#1e1e1e' : '#f9f9f9'
                    } as React.CSSProperties} >
                    <div className="sticky-btns" >
                        <button
                            className="expand-btn"
                            style={{
                                '--expand-btn-bg': DarkMode ? '#333' : '#e0e0e0',
                                '--expand-btn-color': DarkMode ? '#fff' : '#000',
                                '--expand-btn-border': DarkMode ? '#666' : '#ccc'
                            } as React.CSSProperties}
                            onClick={() => {
                                setIsModalOpen(true);
                            }}
                            title="Expand view" >
                            🔍
                        </button>
                    </div>
                    {failedRows.length > 0 && (
                        <div className="mb-2rem">
                            {RenderFailedTable(failedRows)}
                        </div>
                    )}
                    {failedRows.length === 0 && (
                        <p className="italic">No data loaded yet.</p>
                    )}
                </div>
            )}
            {isModalOpen && (
                <div className="modal-overlay" >
                    <div className="modal-btns" >
                        <button
                            className="modal-close-btn"
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
                    </div>
                    <div
                        className="modal-contents"
                        style={{
                            '--modal-content-bg': DarkMode ? '#1e1e1e' : '#fff',
                            '--modal-content-color': DarkMode ? '#e0e0e0' : '#000'
                        } as React.CSSProperties}
                        onClick={e => e.stopPropagation()} >
                        <hr />
                        <div className="mb-2rem">
                            {RenderFailedTable(failedRows)}
                        </div>
                    </div>
                </div>
            )}
        </div>
    );
};

export default UploadFile;