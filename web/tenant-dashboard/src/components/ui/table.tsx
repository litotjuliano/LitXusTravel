import { ReactNode } from "react";

interface TableProps { children: ReactNode; className?: string; }
interface TableHeaderProps { children: ReactNode; className?: string; }
interface TableBodyProps { children: ReactNode; className?: string; }
interface TableRowProps { children: ReactNode; className?: string; onClick?: () => void; }
interface TableCellProps { children: ReactNode; isHeader?: boolean; className?: string; }

const Table: React.FC<TableProps> = ({ children, className = "" }) => (
  <div className="relative w-full overflow-x-auto">
    <table className={`min-w-full ${className}`}>{children}</table>
  </div>
);

const TableHeader: React.FC<TableHeaderProps> = ({ children, className = "" }) => (
  <thead className={className}>{children}</thead>
);

const TableBody: React.FC<TableBodyProps> = ({ children, className = "" }) => (
  <tbody className={className}>{children}</tbody>
);

const TableRow: React.FC<TableRowProps> = ({ children, className = "", onClick }) => (
  <tr className={className} onClick={onClick}>{children}</tr>
);

const TableCell: React.FC<TableCellProps> = ({ children, isHeader = false, className = "" }) => {
  const Tag = isHeader ? "th" : "td";
  return <Tag className={className}>{children}</Tag>;
};

const TableHead: React.FC<{ children: ReactNode; className?: string }> = ({ children, className = "" }) => (
  <th className={`px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider ${className}`}>
    {children}
  </th>
);

export { Table, TableHeader, TableBody, TableRow, TableCell, TableHead };
