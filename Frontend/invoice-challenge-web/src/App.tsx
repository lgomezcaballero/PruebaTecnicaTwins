import type { FormEvent } from 'react'
import { useEffect, useState } from 'react'
import {
  Alert,
  Button,
  Card,
  Col,
  Container,
  Form,
  Row,
  Spinner,
  Table,
} from 'react-bootstrap'

const API_URL = '/api/Invoices'

type Factura = {
  id: number
  invoiceNumber: string
  invoiceDate: string
  customerName: string
  customerDocument?: string | null
  netAmount: number
  vatAmount: number
  totalAmount: number
}

type FormularioFactura = {
  invoiceNumber: string
  invoiceDate: string
  customerName: string
  customerDocument: string
  description: string
  quantity: string
  unitPrice: string
  vatRate: string
}

// Se obtiene la fecha y se acota a yyyy/MM/dd
const fechaActual = new Date().toISOString().slice(0, 10)

const formularioInicial: FormularioFactura = {
  invoiceNumber: '',
  invoiceDate: fechaActual,
  customerName: '',
  customerDocument: '',
  description: '',
  quantity: '1',
  unitPrice: '0',
  vatRate: '21',
}

async function pedirFacturas() {
  const respuesta = await fetch(API_URL)

  if (!respuesta.ok) {
    throw new Error('No se pudieron cargar las facturas')
  }

  return (await respuesta.json()) as Factura[]
}

function App() {
  const [facturas, setFacturas] = useState<Factura[]>([])
  const [formulario, setFormulario] =
    useState<FormularioFactura>(formularioInicial)
  const [cargando, setCargando] = useState(true)
  const [guardando, setGuardando] = useState(false)
  const [mensaje, setMensaje] = useState('')
  const [error, setError] = useState('')

  useEffect(() => {
    // Evita actualizar el estado si la pantalla se desmonta antes de que responda la api
    let montado = true

    pedirFacturas()
      .then((datos) => {
        if (montado) setFacturas(datos)
      })
      .catch(() => {
        if (montado) setError('No se pudo conectar con la API')
      })
      .finally(() => {
        if (montado) setCargando(false)
      })

    return () => {
      montado = false
    }
  }, [])

  async function cargarFacturas() {
    setCargando(true)
    setError('')

    try {
      const datos = await pedirFacturas()
      setFacturas(datos)
    } catch {
      setError('No se pudo conectar con la API')
    } finally {
      setCargando(false)
    }
  }

  function cambiarCampo(campo: keyof FormularioFactura, valor: string) {
    setFormulario({
      ...formulario,
      [campo]: valor,
    })
  }

  async function guardarFactura(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    setGuardando(true)
    setMensaje('')
    setError('')

    // El formulario guarda strings porque vienen de inputs, la api espera numeros en el item
    const nuevaFactura = {
      invoiceNumber: formulario.invoiceNumber,
      invoiceDate: formulario.invoiceDate,
      customerName: formulario.customerName,
      customerDocument: formulario.customerDocument || null,
      items: [
        {
          description: formulario.description,
          quantity: Number(formulario.quantity),
          unitPrice: Number(formulario.unitPrice),
          vatRate: Number(formulario.vatRate),
        },
      ],
    }

    try {
      const respuesta = await fetch(API_URL, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(nuevaFactura),
      })

      if (!respuesta.ok) {
        const body = await respuesta.json().catch(() => null)
        throw new Error(body?.message || 'No se pudo crear la factura')
      }

      setFormulario({ ...formularioInicial, invoiceDate: fechaActual })
      setMensaje('Factura creada correctamente')
      await cargarFacturas()
    } catch (err) {
      if (err instanceof Error) {
        setError(err.message)
      } else {
        setError('No se pudo crear la factura')
      }
    } finally {
      setGuardando(false)
    }
  }

  return (
    <Container className="py-4">
      <div className="d-flex justify-content-between align-items-start mb-3">
        <div>
          <h1>Facturas</h1>
        </div>
        <Button variant="secondary" onClick={() => void cargarFacturas()}>
          Actualizar
        </Button>
      </div>

      {error !== '' && (
        <Alert variant="danger" className="py-2">
          {error}
        </Alert>
      )}

      {mensaje !== '' && (
        <Alert variant="success" className="py-2">
          {mensaje}
        </Alert>
      )}

      <Card className="mb-3">
        <Card.Body>
          <Card.Title>Nueva factura</Card.Title>

          <Form onSubmit={guardarFactura}>
            <Row className="g-3">
              <Col md={3}>
                <Form.Group>
                  <Form.Label>Numero</Form.Label>
                  <Form.Control
                    required
                    value={formulario.invoiceNumber}
                    onChange={(e) => cambiarCampo('invoiceNumber', e.target.value)}
                  />
                </Form.Group>
              </Col>

              <Col md={3}>
                <Form.Group>
                  <Form.Label>Fecha</Form.Label>
                  <Form.Control
                    required
                    type="date"
                    value={formulario.invoiceDate}
                    onChange={(e) => cambiarCampo('invoiceDate', e.target.value)}
                  />
                </Form.Group>
              </Col>

              <Col md={3}>
                <Form.Group>
                  <Form.Label>Cliente</Form.Label>
                  <Form.Control
                    required
                    value={formulario.customerName}
                    onChange={(e) => cambiarCampo('customerName', e.target.value)}
                  />
                </Form.Group>
              </Col>

              <Col md={3}>
                <Form.Group>
                  <Form.Label>Documento</Form.Label>
                  <Form.Control
                    value={formulario.customerDocument}
                    onChange={(e) =>
                      cambiarCampo('customerDocument', e.target.value)
                    }
                  />
                </Form.Group>
              </Col>
            </Row>

            <h5 className="mt-4 mb-3">Item</h5>

            <Row className="g-3">
              <Col md={6}>
                <Form.Group>
                  <Form.Label>Descripcion</Form.Label>
                  <Form.Control
                    required
                    value={formulario.description}
                    onChange={(e) => cambiarCampo('description', e.target.value)}
                  />
                </Form.Group>
              </Col>

              <Col md={2}>
                <Form.Group>
                  <Form.Label>Cantidad</Form.Label>
                  <Form.Control
                    required
                    min="0.01"
                    step="0.01"
                    type="number"
                    value={formulario.quantity}
                    onChange={(e) => cambiarCampo('quantity', e.target.value)}
                  />
                </Form.Group>
              </Col>

              <Col md={2}>
                <Form.Group>
                  <Form.Label>Precio</Form.Label>
                  <Form.Control
                    required
                    min="0"
                    step="0.01"
                    type="number"
                    value={formulario.unitPrice}
                    onChange={(e) => cambiarCampo('unitPrice', e.target.value)}
                  />
                </Form.Group>
              </Col>

              <Col md={2}>
                <Form.Group>
                  <Form.Label>IVA %</Form.Label>
                  <Form.Control
                    required
                    min="0"
                    step="0.01"
                    type="number"
                    value={formulario.vatRate}
                    onChange={(e) => cambiarCampo('vatRate', e.target.value)}
                  />
                </Form.Group>
              </Col>
            </Row>

            <Button className="mt-3" type="submit" disabled={guardando}>
              {guardando ? 'Guardando...' : 'Guardar'}
            </Button>
          </Form>
        </Card.Body>
      </Card>

      <Card>
        <Card.Body>
          <Card.Title>Listado</Card.Title>

          {cargando && (
            <div>
              <Spinner animation="border" size="sm" /> Cargando...
            </div>
          )}

          {!cargando && facturas.length === 0 && (
            <p className="mb-0">No hay facturas para mostrar.</p>
          )}

          {!cargando && facturas.length > 0 && (
            <div className="table-responsive">
              <Table bordered hover size="sm" className="mb-0">
                <thead>
                  <tr>
                    <th>#</th>
                    <th>Fecha</th>
                    <th>Cliente</th>
                    <th className="text-end">Neto</th>
                    <th className="text-end">IVA</th>
                    <th className="text-end">Total</th>
                  </tr>
                </thead>
                <tbody>
                  {facturas.map((factura) => (
                    <tr key={factura.id}>
                      <td>{factura.invoiceNumber}</td>
                      <td>{factura.invoiceDate?.slice(0, 10)}</td>
                      <td>
                        {factura.customerName}
                        {factura.customerDocument
                          ? ` (${factura.customerDocument})`
                          : ''}
                      </td>
                      <td className="text-end">
                        {factura.netAmount.toFixed(2)}
                      </td>
                      <td className="text-end">{factura.vatAmount.toFixed(2)}</td>
                      <td className="text-end">
                        {factura.totalAmount.toFixed(2)}
                      </td>
                    </tr>
                  ))}
                </tbody>
              </Table>
            </div>
          )}
        </Card.Body>
      </Card>
    </Container>
  )
}

export default App